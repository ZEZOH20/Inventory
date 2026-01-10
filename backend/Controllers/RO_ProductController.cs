using Inventory.Data.DbContexts;
using Inventory.DTO.RO_ProductDto.Requests;
using Inventory.DTO.RO_ProductDto.Validators;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class RO_ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly RO_ProductCreateDTOValidator _CreateDTOValidator;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        public RO_ProductController(
            SqlDbContext conn,
            RO_ProductCreateDTOValidator CreateDTOValidator,
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
                var products = _conn.RO_Product.Select(s => s)
                    .ToList();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get release order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] RO_ProductCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            //validation
            try
            {
                //step 1 : delete warehouse_product 
                var deleteResponse = _Warehouse_ProductService.Delete(dto.WarehouseProduct_Id);
                Warehouse_Product warehouse_product = deleteResponse.Data;


                //step 2 : add deleted product details into into RO_Product table
                AddDeletedProductDetails(warehouse_product, dto.RO_Number);

                _conn.SaveChanges();

                return Ok($"warehouse product {dto.WarehouseProduct_Id} Release successfully with release Order details ");

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                //throw;
                return BadRequest("Can't Create Release Order" + ex.Message);
            }
        }

        void AddDeletedProductDetails(Warehouse_Product wp, int Number)
        {
            _conn.RO_Product.Add(new RO_Product
            {
                RO_Amount = wp.Total_Amount,
                RO_Unit = wp.Product?.Unit ?? "N/A", // fallback if somehow null
                RO_Price = wp.Total_Price,
                RO_Number = Number,
                Product_Code = wp.Product_Code
            });
        }


    }
}
