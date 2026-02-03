using backend.DTO.TransferOrderDto.Requests;
using backend.DTO.TransferOrderDto.Validations;
using Inventory.Data.DbContexts;
using Inventory.Models;
using Inventory.Services.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class TransferOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly TransferOrderCreateDTOValidator _CreateDTOValidator;
        readonly ICurrentUser _currentUser;
        public TransferOrderController(
            SqlDbContext conn,
            TransferOrderCreateDTOValidator CreateDTOValidator,
            ICurrentUser currentUser
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
            _currentUser = currentUser;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get accessible warehouse IDs based on user role
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(_currentUser.UserId, _currentUser.UserRole);

                var orders = await _conn.Transfer_Orders
                    .Where(to => accessibleWarehouseIds.Contains(to.From) ||
                                 accessibleWarehouseIds.Contains(to.To))
                    .Select(s => s)
                    .ToListAsync();

                return Ok(orders);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get Transfer orders" + ex.Message);
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
                var transferOrder = new Transfer_Order
                {
                    Supplier_ID = dto.Supplier_ID,
                    From = dto.From,
                    To = dto.To,
                    T_Date = DateTime.UtcNow,
                };

                _conn.Transfer_Orders.Add(transferOrder);
                _conn.SaveChanges();

                return Ok(new {
                     message = "Transfer Order Created Successfully",
                     orderId = transferOrder.Number 
                     });

            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                return BadRequest($"Can't Create Transfer Orders: {ex.Message}. Inner: {innerMessage}");
            }
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
