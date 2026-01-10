using Inventory.DTO.ReportsDto.Requests;
using Inventory.DTO.ReportsDto.Validations;
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
    public class ReportsController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly ICurrentUser _currentUser;

        public ReportsController(IReportingService reportingService, ICurrentUser currentUser)
        {
            _reportingService = reportingService;
            _currentUser = currentUser;
        }

        [HttpGet("supply-orders")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> GetSupplyOrdersReport([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _reportingService.GetSupplyOrdersReportAsync(request, _currentUser.UserId, _currentUser.UserRole);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("release-orders")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> GetReleaseOrdersReport([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _reportingService.GetReleaseOrdersReportAsync(request, _currentUser.UserId, _currentUser.UserRole);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("transfer-orders")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> GetTransferOrdersReport([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _reportingService.GetTransferOrdersReportAsync(request, _currentUser.UserId, _currentUser.UserRole);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("financial-summary")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> GetFinancialSummaryReport([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var result = await _reportingService.GetFinancialSummaryReportAsync(request, _currentUser.UserId, _currentUser.UserRole);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("supply-orders/pdf")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> ExportSupplyOrdersReportPdf([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            try
            {
                var pdfBytes = await _reportingService.ExportSupplyOrdersReportPdfAsync(request, _currentUser.UserId, _currentUser.UserRole);
                return File(pdfBytes, "application/pdf", "SupplyOrdersReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Inventory.Shares.Response.Failure($"Error generating PDF: {ex.Message}"));
            }
        }

        [HttpGet("release-orders/pdf")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> ExportReleaseOrdersReportPdf([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            try
            {
                var pdfBytes = await _reportingService.ExportReleaseOrdersReportPdfAsync(request, _currentUser.UserId, _currentUser.UserRole);
                return File(pdfBytes, "application/pdf", "ReleaseOrdersReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Inventory.Shares.Response.Failure($"Error generating PDF: {ex.Message}"));
            }
        }

        [HttpGet("transfer-orders/pdf")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> ExportTransferOrdersReportPdf([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            try
            {
                var pdfBytes = await _reportingService.ExportTransferOrdersReportPdfAsync(request, _currentUser.UserId, _currentUser.UserRole);
                return File(pdfBytes, "application/pdf", "TransferOrdersReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Inventory.Shares.Response.Failure($"Error generating PDF: {ex.Message}"));
            }
        }

        [HttpGet("financial-summary/pdf")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> ExportFinancialSummaryReportPdf([FromQuery] ReportRequestDto request)
        {
            var validator = new ReportRequestDtoValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(Inventory.Shares.Response.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            try
            {
                var pdfBytes = await _reportingService.ExportFinancialSummaryReportPdfAsync(request, _currentUser.UserId, _currentUser.UserRole);
                return File(pdfBytes, "application/pdf", "FinancialSummaryReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Inventory.Shares.Response.Failure($"Error generating PDF: {ex.Message}"));
            }
        }
    }
}