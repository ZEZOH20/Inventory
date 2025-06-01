using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Inventory.DTO.UserDto.Responses;
using Inventory.Data.DbContexts;
using Inventory.DTO.WarehouseDto.Requests;
using Inventory.DTO.WarehouseDto.Validations;
using Microsoft.EntityFrameworkCore;
using Inventory.DTO.WarehouseDto.Responses;
using Inventory.DTO.Warehouse_ProductDto.Responses;
using Inventory.DTO.ProductDto.Responses;
namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WarehouseController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly WarehouseCreateDTOValidator _CreateDTOValidator;
        readonly WarehouseUpdateDTOValidator _UpdateDTOValidator;
        public WarehouseController(
            SqlDbContext conn
            ,WarehouseCreateDTOValidator CreateDTOValidator,
              WarehouseUpdateDTOValidator UpdateDTOValidator
            )
        {
            _CreateDTOValidator = CreateDTOValidator;
            _UpdateDTOValidator = UpdateDTOValidator;
            _conn = conn;
           
        }
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var warhouses = _conn.Warehouses
                        .Select(w => new WarehouseResponseDTO
                        {
                            Number = w.Number,
                            Name = w.Name,
                            Region = w.Region,
                            City = w.City,
                            Street = w.Street,
                            Manager = new UserResponseDTO
                            {
                                Id = w.Manager.Id,
                                Name = w.Manager.Name,
                                Phone = w.Manager.Phone,
                                Mail = w.Manager.Mail
                            },
                            Warehouse_Products = w.Warehouse_Products.Select(wp => new Warehouse_ProductResponseDTO
                            {
                                Id=wp.Id,
                                War_Number= wp.War_Number,
                                Product_Code = wp.Product_Code,
                                Supplier_ID = wp.Supplier_ID,
                                Total_Amount = wp.Total_Amount,
                                Total_Price = wp.Total_Price,
                                EXP = wp.EXP,
                                MFD = wp.MFD,
                                SupplierName = wp.Supplier.Name,
                                Product = new ProductResponseDTO
                                {
                                    Name = wp.Product.Name,
                                    Code = wp.Product.Code,
                                    Unit = wp.Product.Unit
                                }
                            }).ToList()
                        })
                        .ToList();

                return Ok(warhouses);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't return Warehouses" + ex.Message);
            }
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody] WarehouseCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {

               // Check if manager already assigned to another warehouse
                string result = CheckWarehouseManaged(dto.ManagerId);
                bool WarehouseManaged = !string.IsNullOrEmpty(result);
                if (WarehouseManaged)
                    return BadRequest(result);


                _conn.Warehouses.Add(new Warehouse
                {
                    Name = dto.Name,
                    Region = dto.Region,
                    City = dto.City,
                    Street = dto.Street,
                    ManagerId = dto.ManagerId
                });
                _conn.SaveChanges();

                return Ok("Warehouse Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Warehouse" + ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateBYId([FromBody] WarehouseUpdateDTO dto)
        {
            //validation
            var validationResult = _UpdateDTOValidator.Validate(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            try
            {
                //check if manager id enterd
                if (string.IsNullOrEmpty(dto.ManagerId))
                     return BadRequest("ManagerId can't be null");
                
                int ManagerId = int.Parse(dto.ManagerId);

                // Check if manager already assigned to another warehouse
                string message = CheckWarehouseManaged(ManagerId);
                bool WarehouseManaged = !string.IsNullOrEmpty(message);
                if (WarehouseManaged)
                    return BadRequest(message);


                var result = UpdateWarehouse(dto);

                return !result ?
                     NotFound($"warehouse Number:  {dto.Number} not found") :
                     Ok("warehouse updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Update Warehouse" + ex.Message);
            }
        }
  
        [HttpDelete("delete/{number}")]
        public IActionResult Delete(int number)
        {
            //validate ID
            if (number <= 0)
                return BadRequest($"the user id {number} can't be zero and should be positive ");

            try
            {
                var Warehouse = _conn.Warehouses.FirstOrDefault(w => w.Number == number);

                if (Warehouse == null)
                    return BadRequest($"the Warehouse Number {number} not found ");

                 _conn.Warehouses.Remove(Warehouse);
                _conn.SaveChanges();

                return Ok("Warehouse deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Can't delete User" + ex.Message);
            }
        }

      string CheckWarehouseManaged(int ManagerId)
        {
            //check if Manager is Exists
            var ManagerExists = _conn.Users.Any(m => m.Id == ManagerId);

            if (!ManagerExists)
                return $"Manager {ManagerId} doesn't exists to Manage warehouse";

            // Check if manager already assigned to another warehouse
            var existingWarehouse = _conn.Warehouses
            .Include(w => w.Manager)
            .FirstOrDefault(w => w.ManagerId == ManagerId);

            if (existingWarehouse != null)
            {
                return $"Manager\n" +
                    $"Id : {existingWarehouse.Manager.Id}\n" +
                    $"Name : {existingWarehouse.Manager.Name} \n" +
                    $"is already Manage warehouse\n" +
                    $"Code : {existingWarehouse.Number}\n" +
                    $"Name :{existingWarehouse.Name}";
            }
            return "";
        }
         bool UpdateWarehouse(WarehouseUpdateDTO dto)
        {
            var warehouse = _conn.Warehouses.FirstOrDefault(w => w.Number == dto.Number);

            if (warehouse == null)
                return false;

            // Update only provided values
            if (!string.IsNullOrEmpty(dto.Name))
                warehouse.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Region))
                warehouse.Region = dto.Region;

            if (!string.IsNullOrEmpty(dto.Street))
                warehouse.Street = dto.Street;

            if (!string.IsNullOrEmpty(dto.City))
                warehouse.City = dto.City;

            if (!string.IsNullOrEmpty(dto.ManagerId))
                warehouse.ManagerId = int.Parse(dto.ManagerId);

            _conn.SaveChanges();

            return true;
        }
    }
}
