using Inventory.Data.DbContexts;
using Inventory.DTO.SupplyOrderDto.Requests;
using Inventory.DTO.SupplyOrderDto.Validations;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class SupplyOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly SupplyOrderCreateDTOValidator _CreateDTOValidator;
        public SupplyOrderController(
            SqlDbContext conn,
            SupplyOrderCreateDTOValidator CreateDTOValidator
            )
        {
            _conn = conn;
            _CreateDTOValidator= CreateDTOValidator;    
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var products = _conn.Supply_Orders.Select(s => s)
                    .ToList();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Supply orders" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] SupplyOrderCreateDTO dto)
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
                    _conn.Supply_Orders.Add(new Supply_Order
                    {
                        Supplier_ID = dto.Supplier_ID,
                        War_Number = dto.War_Number,
                        S_Date = DateTime.UtcNow,
                    });

                _conn.SaveChanges();

                return Ok("Supply Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Supply Orders" + ex.Message);
            }
        }
       



    }
}
