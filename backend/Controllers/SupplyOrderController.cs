using Inventory.Data.DbContexts;
using Inventory.DTO.SupplyOrderDto.Requests;
using Inventory.DTO.SupplyOrderDto.Validations;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Services;
using Inventory.Services.CurrentUser;
using Inventory.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class SupplyOrderController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly SupplyOrderCreateDTOValidator _CreateDTOValidator;
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUser _currentUser;
        readonly IApprovalService _approvalService;
        readonly UserManager<ApplicationUser> _userManager;

        public SupplyOrderController(
            SqlDbContext conn,
            SupplyOrderCreateDTOValidator CreateDTOValidator,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IApprovalService approvalService,
            UserManager<ApplicationUser> userManager
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _approvalService = approvalService;
            _userManager = userManager;
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
        public async Task<IActionResult> Create([FromBody] SupplyOrderCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var userId = _currentUser.UserId;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return BadRequest("User not found");

                var userRoles = await _userManager.GetRolesAsync(user);

                bool isOwner = userRoles.Contains("Owner");
                bool isManager = userRoles.Contains("Manager");

                var order = new Supply_Order
                {
                    Supplier_ID = dto.Supplier_ID,
                    War_Number = dto.War_Number,
                    S_Date = DateTime.UtcNow,
                };

                // Set initial status based on role
                if (isOwner || isManager)
                {
                    // Owners and Managers can auto-approve their orders
                    order.Status = OrderStatus.Approved;
                    order.ApprovedBy = userId;
                    order.ApprovedAt = DateTime.UtcNow;
                    order.ReviewNotes = "Auto-approved by creator";
                }
                else
                {
                    // Employees need approval
                    order.Status = OrderStatus.Pending;
                }

                order.SetCreated(userId);

                _unitOfWork.SupplyOrders.Add(order);
                await _unitOfWork.SaveChangesAsync();

                // If auto-approved, execute inventory changes
                if (order.Status == OrderStatus.Approved)
                {
                    // TODO: Implement inventory update logic for supply orders
                }

                return Ok(new { Message = "Supply Order Created successfully", OrderId = order.Number, Status = order.Status.ToString() });

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Supply Orders" + ex.Message);
            }
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelRequest request)
        {
            var result = await _approvalService.CancelOrderAsync(
                orderId,
                OrderType.Supply,
                request.CancellationReason,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }
    }

    public class CancelRequest
    {
        public string CancellationReason { get; set; } = string.Empty;
    }
}
