using Inventory.Shares;

namespace Inventory.Services
{
    public interface IInventoryReservationService
    {
        Task<Response> ReserveInventoryAsync(int warehouseProductId, double quantity);
        Task<Response> ReleaseReservationAsync(int warehouseProductId, double quantity);
        Task<Response> ReleaseReservationAsync(int orderId, OrderType orderType);
        Task<Response<double>> GetAvailableQuantityAsync(int warehouseProductId);
    }
}