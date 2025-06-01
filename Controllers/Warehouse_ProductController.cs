using Inventory.Data.DbContexts;
using Inventory.DTO.ProductDto.Requests;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.DTO.WarehouseDto.Requests;
using Inventory.DTO.WarehouseDto.Validations;
using Inventory.Models;
using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class Warehouse_ProductController:ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        public Warehouse_ProductController(
            SqlDbContext conn,
            IWarehouse_ProductService Warehouse_ProductService
            ){
            _conn = conn;
            _Warehouse_ProductService = Warehouse_ProductService;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Warehouse_ProductCreateDTO dto)
            => Ok(_Warehouse_ProductService.CreateWarehouse_Product(dto));


        [HttpPut("Update")]
        public IActionResult UpdateBYId([FromBody] Warehouse_ProductUpdateDTO dto)
        {
            var validationResult = dto.Id > 0;

            if (!validationResult)
                return BadRequest($"your Product Id : {dto.Id} in Warehouse  should be positve value");

            try
            {

                var result = UpdateProduct(dto);

                return !result ?
                     NotFound($"Product in Warehouse :  {dto.Id} not found") :
                     Ok("Product in Warehouse updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Update Product in Warehouse" + ex.Message);
            }
        }


        [HttpDelete("delete/{Id}")]
        public IActionResult Delete(int Id)
        {
            //validate ID
            if (Id <= 0)
                return BadRequest($"the Product Id: {Id} in Warehouse can't be zero and should be positive ");

            try
            {
                var Warehouse_Product = _conn.Warehouse_Products.FirstOrDefault(wp => wp.Id == Id);

                if (Warehouse_Product == null)
                    return BadRequest($"the Product Id: {Id} in Warehouse not found ");

                _conn.Warehouse_Products.Remove(Warehouse_Product);
                _conn.SaveChanges();

                return Ok("Product in Warehouse deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Can't delete Product in Warehouse" + ex.Message);
            }
        }


        bool UpdateProduct(Warehouse_ProductUpdateDTO dto)
        {
            var Warehouse_product = _conn.Warehouse_Products.FirstOrDefault(wp => wp.Id == dto.Id);

            if (Warehouse_product == null)
                return false;

            // Update only provided values
            if (dto.Product_Code > 0)
                Warehouse_product.Product_Code = (int) dto.Product_Code;

            if (dto.Supplier_ID > 0)
                Warehouse_product.Product_Code = (int)dto.Supplier_ID;

            if (dto.Total_Amount > 0)
                Warehouse_product.Product_Code = (int)dto.Total_Amount;

            if (dto.Total_Price > 0)
                Warehouse_product.Product_Code = (int)dto.Total_Price;

            if (dto.War_Number > 0)
                Warehouse_product.Product_Code = (int)dto.War_Number;

            if(dto.MFD != null)
                Warehouse_product.MFD = (DateTime) dto.MFD;

            if (dto.EXP != null)
                Warehouse_product.EXP = (DateTime)dto.EXP;

            if (dto.Store_Date != null)
                Warehouse_product.Store_Date = (DateTime)dto.Store_Date;

            _conn.SaveChanges();

            return true;
        }

      
    }
}
