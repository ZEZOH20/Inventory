using Inventory.Data.DbContexts;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.SO_ProductDto.Responses;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Inventory.Services;
using Inventory.Services.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;



namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class SO_ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly SO_ProductCreateDTOValidator _CreateDTOValidator;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        readonly ICurrentUser _currentUser;
        public SO_ProductController(
            SqlDbContext conn,
            SO_ProductCreateDTOValidator CreateDTOValidator,
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
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(_currentUser.UserId, _currentUser.UserRole);

                var products = await _conn.SO_Products
                    .Include(sop => sop.Supply_Order)
                        .ThenInclude(so => so.Warehouse)
                    .Include(sop => sop.Supply_Order)
                        .ThenInclude(so => so.Supplier)
                    .Include(sop => sop.Product)
                    .Where(sop => accessibleWarehouseIds.Contains(sop.Supply_Order.War_Number))
                    .Select(sop => new SO_ProductResponseDTO
                    {
                        Id = sop.Id,
                        SO_Amount = sop.SO_Amount,
                        SO_Unit = sop.SO_Unit,
                        SO_Price = sop.SO_Price,
                        SO_MFD = sop.SO_MFD,
                        SO_EXP = sop.SO_EXP,
                        SO_Number = sop.SO_Number,
                        Product_Code = sop.Product_Code,
                        Product = new Inventory.DTO.ProductDto.Responses.ProductResponseDTO
                        {
                            Code = sop.Product.Code,
                            Name = sop.Product.Name,
                            Unit = sop.Product.Unit,
                            Image = sop.Product.Image
                        },
                        SupplierName = sop.Supply_Order.Supplier.Name,
                        WarehouseName = sop.Supply_Order.Warehouse.Name,
                        S_Date = sop.Supply_Order.S_Date,
                        Status = sop.Supply_Order.Status
                    })
                    .ToListAsync();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Supply order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SO_ProductCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            DateTime expDate = DateTime.Parse(dto.SO_EXP);
            DateTime mfdDate = DateTime.Parse(dto.SO_MFD);
            if (expDate <= mfdDate)
                return BadRequest($"EXP Date : {expDate} \n " +
                    $"can't be less than or equal\n" +
                    $"MFD Date : {mfdDate}");

            //validation
            try
            {
                //step 1 : add product into SO_Product table
                _conn.SO_Products.Add(new SO_Product
                {
                    SO_Amount = dto.SO_Amount,
                    SO_Unit = dto.SO_Unit,
                    SO_Price = dto.SO_Price,
                    SO_MFD = mfdDate,
                    SO_EXP = expDate,
                    SO_Number = dto.SO_Number,
                    Product_Code = dto.Product_Code
                });

                //step 2 : add product to warehouse_products table
                await AutomaticAddProductToWarehouse(dto);

                _conn.SaveChanges();

                return Ok("Supply Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Supply Orders" + ex.Message);
            }
        }

        async Task<bool> AutomaticAddProductToWarehouse(SO_ProductCreateDTO dto)
        {
            var SupplyOrder = _conn.Supply_Orders.FirstOrDefault(so => so.Number == dto.SO_Number);

            if (SupplyOrder == null)
                return false;

            Warehouse_ProductCreateDTO wp_dto = new Warehouse_ProductCreateDTO
            {
                War_Number = SupplyOrder.War_Number,
                Product_Code = dto.Product_Code,
                Supplier_ID = SupplyOrder.Supplier_ID,
                MFD = dto.SO_MFD,
                EXP = dto.SO_EXP,
                Amount = dto.SO_Amount,
                Price = dto.SO_Price,
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
