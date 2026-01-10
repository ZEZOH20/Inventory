using backend.DTO.TO_ProductDto.Requests;
using backend.DTO.TO_ProductDto.Validators;
using Inventory.Models;
using Inventory.Data.DbContexts;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        public TO_ProductController(
            SqlDbContext conn,
            TO_ProductCreateDTOValidator CreateDTOValidator,
            IWarehouse_ProductService Warehouse_ProductService
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
            _Warehouse_ProductService = Warehouse_ProductService;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var products = _conn.TO_Products.ToList();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Transfer order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] TO_ProductCreateDTO dto)
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


                //step 2 fetch existing product 
                var existingProduct = _Warehouse_ProductService.ProductExistInWarehouse(TransferOrder.Supplier_ID, dto.Product_Code, TransferOrder.From, mfdDate, expDate);


                if (existingProduct != null)
                {

                    //step 3 : add product into TO_Product table
                    _conn.TO_Products.Add(new TO_Product
                    {
                        TO_Amount = dto.TO_Amount,
                        TO_Unit = existingProduct.Product.Unit,
                        TO_Price = existingProduct.Total_Price,
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

                        //step 2 : add product to warehouse_products table
                        AutomaticAddProductToWarehouse(dto, TransferOrder, existingProduct);
                    }
                    else
                    {
                        var deleteResponse = _Warehouse_ProductService.Delete(existingProduct.Id);
                        if (!deleteResponse.IsSuccess)
                            return BadRequest(deleteResponse.Message);
                        AutomaticAddProductToWarehouse(dto, TransferOrder, existingProduct);
                    }
                }
                else
                {
                    return BadRequest("Can't Transfer Order something wrong in inputs data check exists : \n" +
                        "supplier Id  \n" +
                        "warehouse number \n" +
                        "product code \n" +
                        "amount > product total amount in warehouse \n " +
                        "product with MFD and EXP exists");
                }

                // _conn.SaveChanges();

                return Ok("Transfer Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Transfer Orders" + ex.Message);
            }
        }

        bool AutomaticAddProductToWarehouse(TO_ProductCreateDTO dto, Transfer_Order? TransferOrder, Warehouse_Product existingProduct)
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
                Price = existingProduct.Total_Price,
            };

            //create actual product 
            var createResponse = _Warehouse_ProductService.CreateWarehouse_Product(wp_dto);
            if (!createResponse.IsSuccess)
                return false;

            return true;
        }


    }
}
