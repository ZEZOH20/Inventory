using Inventory.DTO.ReportsDto.Requests;
using Inventory.DTO.ReportsDto.Responses;
using Inventory.Shares;

namespace Inventory.Services
{
    public interface IReportingService
    {
        Task<Response<List<SupplyOrderReportDto>>> GetSupplyOrdersReportAsync(ReportRequestDto request, string userId, string userRole);
        Task<Response<List<ReleaseOrderReportDto>>> GetReleaseOrdersReportAsync(ReportRequestDto request, string userId, string userRole);
        Task<Response<List<TransferOrderReportDto>>> GetTransferOrdersReportAsync(ReportRequestDto request, string userId, string userRole);
        Task<Response<FinancialSummaryDto>> GetFinancialSummaryReportAsync(ReportRequestDto request, string userId, string userRole);

        // PDF export methods
        Task<byte[]> ExportSupplyOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole);
        Task<byte[]> ExportReleaseOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole);
        Task<byte[]> ExportTransferOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole);
        Task<byte[]> ExportFinancialSummaryReportPdfAsync(ReportRequestDto request, string userId, string userRole);
    }
}