using Inventory.Data.DbContexts;
using Inventory.DTO.ProductDto.Requests;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.DTO.WarehouseDto.Requests;
using Inventory.DTO.WarehouseDto.Validations;
using Inventory.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class Warehouse_ProductController:ControllerBase
    {
        readonly SqlDbContext _conn;
        public Warehouse_ProductController(SqlDbContext conn)
        {
            _conn = conn;   
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Warehouse_ProductCreateDTO dto)
        {

            //validation
            if (!dto.Valid())
                return BadRequest($"check: \n" +
                    $"Product_Code,\n" +
                    $"War_Number,\n" +
                    $"Amount,\n" +
                    $"Price,\n" +
                    $"greater than > 0 \n\n" +
                    $"MFD , EXP , EXP are Date Type");

            if (!CreationIsValid(dto))
                return BadRequest($"Supplier_ID or \n " +
                    $"Product_Code or \n " +
                    $"Warehouse_Number \n  " +
                    "can't found check them and try later");

                // Parse and validate dates
                DateTime mfdDate = DateTime.Parse(dto.MFD);
                DateTime expDate = DateTime.Parse(dto.EXP);
            if (expDate <= mfdDate)
                return BadRequest($"EXP Date : {expDate} \n " +
                    $"can't be less than or equal\n" +
                    $"MFD Date : {mfdDate}");
            //validation

            try
            {
                var existingProduct = ProductExistInWarehouse(dto);

                if (existingProduct is not null)
                {
                    existingProduct.Total_Amount += dto.Amount;
                    existingProduct.Total_Price += dto.Amount * dto.Price;
                }
                else
                {
                  

                    _conn.Warehouse_Products.Add(new Warehouse_Product
                    {
                        War_Number = dto.War_Number,
                        Product_Code = dto.Product_Code,
                        Supplier_ID = dto.Supplier_ID,
                        MFD = mfdDate,
                        EXP = expDate,
                        Store_Date = DateTime.UtcNow,
                        Total_Amount = dto.Amount,
                        Total_Price = dto.Amount * dto.Price,
                    });
                }
                _conn.SaveChanges();

                return Ok("Product in Warehouse Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Product in Warehouse" + ex.Message);
            }
        }
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

        bool CreationIsValid(Warehouse_ProductCreateDTO dto)
        {
            var SupplierExists = _conn.Suppliers.Any(s => s.Id == dto.Supplier_ID);
            var ProductExists = _conn.Products.Any(p => p.Code == dto.Product_Code);
            var WarehouseExists = _conn.Warehouses.Any(w => w.Number == dto.War_Number);

            if (!SupplierExists ||
                !ProductExists ||
                !WarehouseExists
                )
                return false;

            return true;
        }

        Warehouse_Product? ProductExistInWarehouse(Warehouse_ProductCreateDTO dto)
           => _conn.Warehouse_Products.FirstOrDefault(wp =>
              wp.Supplier_ID == dto.Supplier_ID &&
              wp.Product_Code== dto.Product_Code &&
              wp.War_Number==dto.War_Number &&
              wp.EXP == DateTime.Parse(dto.MFD) &&
              wp.MFD == DateTime.Parse(dto.MFD)
           );


    }
}
