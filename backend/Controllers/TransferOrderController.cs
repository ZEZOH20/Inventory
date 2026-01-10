using backend.DTO.TransferOrderDto.Requests;
using backend.DTO.TransferOrderDto.Validations;
using Inventory.Data.DbContexts;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class TransferOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly TransferOrderCreateDTOValidator _CreateDTOValidator;
        public TransferOrderController(
            SqlDbContext conn,
            TransferOrderCreateDTOValidator CreateDTOValidator
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
                var orders = _conn.Transfer_Orders.Select(s => s)
                    .ToList();

                return Ok(orders);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Supply orders" + ex.Message);
            }
        }

        [HttpPost("create")]
        //TransferOrderCreateDto
        public IActionResult Create([FromBody] TransferOrderCreateDto dto)
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
                _conn.Transfer_Orders.Add(new Transfer_Order
                {
                    Supplier_ID = dto.Supplier_ID,
                    From = dto.From,
                    To = dto.To,
                    T_Date = DateTime.UtcNow,
                });

                _conn.SaveChanges();

                return Ok("Transfer Order Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Transfer Orders" + ex.Message);
            }
        }




    }
}
