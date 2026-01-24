using Inventory.Data.DbContexts;
using Inventory.DTO.ReleaseOrderDto.Requests;
using Inventory.DTO.ReleaseOrderDto.Validators;
using Inventory.DTO.SupplyOrderDto.Requests;
using Inventory.DTO.SupplyOrderDto.Validations;
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
    public class ReleaseOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly ReleaseOrderCreateDTOValidator _CreateDTOValidator;
        readonly ICurrentUser _currentUser;
        public ReleaseOrderController(
            SqlDbContext conn,
            ReleaseOrderCreateDTOValidator CreateDTOValidator,
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

                var products = await _conn.Release_Orders
                    .Where(ro => accessibleWarehouseIds.Contains(ro.War_Number))
                    .Select(s => s)
                    .ToListAsync();

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
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                return BadRequest($"Can't Create Release Order: {ex.Message}. Inner: {innerMessage}");
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
