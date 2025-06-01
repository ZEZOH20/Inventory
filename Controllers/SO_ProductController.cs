using Inventory.Data.DbContexts;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;



namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class SO_ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly SO_ProductCreateDTOValidator _CreateDTOValidator;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        public SO_ProductController(
            SqlDbContext conn,
            SO_ProductCreateDTOValidator CreateDTOValidator,
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
                var products = _conn.SO_Products.Select(s => s)
                    .ToList();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Supply order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] SO_ProductCreateDTO dto)
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
                        SO_MFD  = mfdDate,
                         SO_EXP =expDate,
                        SO_Number = dto.SO_Number,
                        Product_Code = dto.Product_Code
                    });

                //step 2 : add product to warehouse_products table
                AutomaticAddProductToWarehouse(dto);

                _conn.SaveChanges();

                return Ok("Supply Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Supply Orders" + ex.Message);
            }
        }

        bool AutomaticAddProductToWarehouse(SO_ProductCreateDTO dto)
        {
            var SupplyOrder = _conn.Supply_Orders.FirstOrDefault(so=>so.Number==dto.SO_Number);

            if( SupplyOrder == null ) 
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
            _Warehouse_ProductService.CreateWarehouse_Product(wp_dto);

            return true;
        }


    }
}
