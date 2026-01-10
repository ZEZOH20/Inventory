using Inventory.Services;
using Inventory.Services.CurrentUser;
using Inventory.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        private readonly ICurrentUser _currentUser;

        public ApprovalController(IApprovalService approvalService, ICurrentUser currentUser)
        {
            _approvalService = approvalService;
            _currentUser = currentUser;
        }

        [HttpPost("supply-orders/{orderId}/approve")]
        public async Task<IActionResult> ApproveSupplyOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.ApproveOrderAsync(
                orderId,
                OrderType.Supply,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpPost("supply-orders/{orderId}/reject")]
        public async Task<IActionResult> RejectSupplyOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.RejectOrderAsync(
                orderId,
                OrderType.Supply,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpPost("release-orders/{orderId}/approve")]
        public async Task<IActionResult> ApproveReleaseOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.ApproveOrderAsync(
                orderId,
                OrderType.Release,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpPost("release-orders/{orderId}/reject")]
        public async Task<IActionResult> RejectReleaseOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.RejectOrderAsync(
                orderId,
                OrderType.Release,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpPost("transfer-orders/{orderId}/approve")]
        public async Task<IActionResult> ApproveTransferOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.ApproveOrderAsync(
                orderId,
                OrderType.Transfer,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpPost("transfer-orders/{orderId}/reject")]
        public async Task<IActionResult> RejectTransferOrder(int orderId, [FromBody] ApprovalRequest request)
        {
            var result = await _approvalService.RejectOrderAsync(
                orderId,
                OrderType.Transfer,
                request.ReviewNotes,
                _currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Message);
        }

        [HttpGet("pending-orders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _approvalService.GetPendingOrdersAsync(_currentUser.UserId);

            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode, result.Message);

            return Ok(result.Data);
        }
    }

    public class ApprovalRequest
    {
        public string ReviewNotes { get; set; } = string.Empty;
    }
}