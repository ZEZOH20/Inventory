using backend.DTO.TO_ProductDto.Requests;
using backend.DTO.TO_ProductDto.Validators;
using backend.DTO.TO_ProductDto.Responses;
using Inventory.DTO.ProductDto.Responses;
using Inventory.Models;
using Inventory.Data.DbContexts;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Services;
using Inventory.Services.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Migrations;



namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class TO_ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly TO_ProductCreateDTOValidator _CreateDTOValidator;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        readonly ICurrentUser _currentUser;
        public TO_ProductController(
            SqlDbContext conn,
            TO_ProductCreateDTOValidator CreateDTOValidator,
            IWarehouse_ProductService Warehouse_ProductService,
            ICurrentUser currentUser
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
            _Warehouse_ProductService = Warehouse_ProductService;
            _currentUser = currentUser;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get accessible warehouse IDs based on user role
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(_currentUser.UserId ?? string.Empty, _currentUser.UserRole ?? string.Empty);

                var products = await _conn.TO_Products
                    .Include(top => top.Transfer_Order)
                        .ThenInclude(to => to.Supplier)
                    .Include(top => top.Transfer_Order)
                        .ThenInclude(to => to.FromWarehouse)
                    .Include(top => top.Transfer_Order)
                        .ThenInclude(to => to.ToWarehouse)
                    .Include(top => top.Product)
                    .Where(top => accessibleWarehouseIds.Contains(top.Transfer_Order.From) ||
                                  accessibleWarehouseIds.Contains(top.Transfer_Order.To))
                    .Select(top => new TO_ProductResponseDTO
                    {
                        Id = top.Id,
                        TO_Amount = top.TO_Amount,
                        TO_Unit = top.TO_Unit,
                        TO_Price = top.TO_Price,
                        TO_MFD = top.TO_MFD,
                        TO_EXP = top.TO_EXP,
                        TO_Number = top.TO_Number,
                        Product_Code = top.Product_Code,
                        Product = new ProductResponseDTO
                        {
                            Code = top.Product.Code,
                            Name = top.Product.Name,
                            Unit = top.Product.Unit,
                            Image = top.Product.Image
                        },
                        SupplierName = top.Transfer_Order.Supplier.Name,
                        FromWarehouseName = top.Transfer_Order.FromWarehouse.Name,
                        ToWarehouseName = top.Transfer_Order.ToWarehouse.Name,
                        T_Date = top.Transfer_Order.T_Date,
                        Status = top.Transfer_Order.Status
                    })
                    .ToListAsync();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Transfer order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TO_ProductCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            DateTime expDate = DateTime.Parse(dto.TO_EXP);
            DateTime mfdDate = DateTime.Parse(dto.TO_MFD);
            if (expDate <= mfdDate)
                return BadRequest($"EXP Date : {expDate} \n " +
                    $"can't be less than or equal\n" +
                    $"MFD Date : {mfdDate}");

            //validation
            try
            {
                //step 1 get TransferOrder
                var TransferOrder = _conn.Transfer_Orders.FirstOrDefault(to => to.Number == dto.TO_Number);

                if (TransferOrder == null)
                {
                    return BadRequest("Transfer order not found");
                }

                //step 2 fetch existing product 
                var existingProduct = _Warehouse_ProductService.ProductExistInWarehouse(TransferOrder.Supplier_ID, dto.Product_Code, TransferOrder.From, mfdDate, expDate);


                if (existingProduct != null)
                {
                    // Calculate unit price
                    double unitPrice = existingProduct.Total_Price / existingProduct.Total_Amount;

                    //step 3 : add product into TO_Product table
                    _conn.TO_Products.Add(new TO_Product
                    {
                        TO_Amount = dto.TO_Amount,
                        TO_Unit = existingProduct.Product.Unit,
                        TO_Price = unitPrice * dto.TO_Amount,
                        TO_MFD = mfdDate,
                        TO_EXP = expDate,
                        TO_Number = dto.TO_Number,
                        Product_Code = dto.Product_Code
                    });


                    //step 4 : check : -
                    //  T_amount > amount  in warehouse_product then -> return "can't transfer"
                    //  T_amount < amount  in warehouse_product then -> decrease amount in warehouse(update) , insert into new warehouse , insert into TO_product
                    //  T_amount == amount in warehouse_product then -> remove warehouse_product , insert into new warehouse , insert into TO_product

                    if (dto.TO_Amount > existingProduct.Total_Amount)
                    {
                        return BadRequest("can't transfer Product");
                    }
                    else if (dto.TO_Amount < existingProduct.Total_Amount)
                    {
                        existingProduct.Total_Amount -= dto.TO_Amount;
                        existingProduct.Total_Price -= unitPrice * dto.TO_Amount;

                        //step 2 : add product to warehouse_products table
                        await AutomaticAddProductToWarehouse(dto, TransferOrder, unitPrice);
                    }
                    else
                    {
                        var deleteResponse = await _Warehouse_ProductService.Delete(existingProduct.Id);
                        if (!deleteResponse.IsSuccess)
                            return BadRequest(deleteResponse.Message);
                        await AutomaticAddProductToWarehouse(dto, TransferOrder, unitPrice);
                    }
                }
                else
                {
                    return BadRequest($"Cannot create Transfer Order product. The product was not found in the warehouse with the following details:\n" +
                        $"- Supplier ID: {TransferOrder.Supplier_ID}\n" +
                        $"- Product Code: {dto.Product_Code}\n" +
                        $"- Warehouse Number: {TransferOrder.From}\n" +
                        $"- MFD Date: {mfdDate:yyyy-MM-dd}\n" +
                        $"- EXP Date: {expDate:yyyy-MM-dd}\n" +
                        "Please ensure the product exists in the source warehouse with matching details.");
                }

                _conn.SaveChanges();

                return Ok("Transfer Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Transfer Orders" + ex.Message);
            }
        }

        async Task<bool> AutomaticAddProductToWarehouse(TO_ProductCreateDTO dto, Transfer_Order? TransferOrder, double unitPrice)
        {

            if (TransferOrder == null)
                return false;

            Warehouse_ProductCreateDTO wp_dto = new Warehouse_ProductCreateDTO
            {
                War_Number = TransferOrder.To,
                Product_Code = dto.Product_Code,
                Supplier_ID = TransferOrder.Supplier_ID,
                MFD = dto.TO_MFD,
                EXP = dto.TO_EXP,
                Amount = dto.TO_Amount,
                Price = unitPrice,
            };

            //create actual product 
            var createResponse = await _Warehouse_ProductService.CreateWarehouse_Product(wp_dto);
            if (!createResponse.IsSuccess)
                return false;

            return true;
        }

        private async Task<List<int>> GetAccessibleWarehouseIdsAsync(string userId, string userRole)
        {
            if (userRole == "Owner")
            {
                // Owners can access only warehouses they created
                return await _conn.Warehouses.Where(w => w.CreatedBy == userId).Select(w => w.Number).ToListAsync();
            }
            else if (userRole == "Manager")
            {
                // Managers can only access their assigned warehouse
                var user = await _conn.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user?.WarehouseId.HasValue == true ? new List<int> { user.WarehouseId.Value } : new List<int>();
            }
            else
            {
                // Employees have no warehouse access
                return new List<int>();
            }
        }


    }
}
