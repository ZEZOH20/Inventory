using Inventory.Shares;

namespace Inventory.Services
{
    public interface IApprovalService
    {
        Task<Response> ApproveOrderAsync(int orderId, OrderType orderType, string reviewNotes, string approverId);
        Task<Response> RejectOrderAsync(int orderId, OrderType orderType, string reviewNotes, string rejectorId);
        Task<Response<List<PendingOrderDto>>> GetPendingOrdersAsync(string userId, int? warehouseId = null);
        Task<Response> CancelOrderAsync(int orderId, OrderType orderType, string cancellationReason, string userId);
    }

    public enum OrderType
    {
        Supply,
        Release,
        Transfer
    }

    public class PendingOrderDto
    {
        public int OrderId { get; set; }
        public OrderType OrderType { get; set; }
        public string OrderNumber { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal TotalValue { get; set; }
        public string Status { get; set; }
    }
}