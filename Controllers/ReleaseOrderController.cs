using Inventory.Data.DbContexts;
using Inventory.DTO.ReleaseOrderDto.Requests;
using Inventory.DTO.ReleaseOrderDto.Validators;
using Inventory.DTO.SupplyOrderDto.Requests;
using Inventory.DTO.SupplyOrderDto.Validations;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ReleaseOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly ReleaseOrderCreateDTOValidator _CreateDTOValidator;
        public ReleaseOrderController(
            SqlDbContext conn,
            ReleaseOrderCreateDTOValidator CreateDTOValidator
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var products = _conn.Release_Orders.Select(s => s)
                    .ToList();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Release orders" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] ReleaseOrderCreateDTO dto)
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
                _conn.Release_Orders.Add(new Release_Order
                {
                    Customer_ID = dto.Customer_ID,
                    War_Number = dto.War_Number,
                    R_Date = DateTime.UtcNow,
                });

                _conn.SaveChanges();

                return Ok("Release Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Release Order" + ex.Message);
            }
        }




    }
}
