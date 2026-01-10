using Inventory.Data.DbContexts;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;

namespace Inventory.Services
{
    public class InventoryReservationService : IInventoryReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryReservationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> ReserveInventoryAsync(int warehouseProductId, double quantity)
        {
            try
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts.FirstOrDefaultAsync(wp => wp.Id == warehouseProductId);
                if (warehouseProduct == null)
                    return Response.Failure("Warehouse product not found", System.Net.HttpStatusCode.NotFound);

                if (warehouseProduct.AvailableQuantity < quantity)
                    return Response.Failure("Insufficient available inventory", System.Net.HttpStatusCode.BadRequest);

                warehouseProduct.ReservedQuantity += quantity;
                warehouseProduct.SetUpdated("system"); // TODO: Get current user

                await _unitOfWork.SaveChangesAsync();
                return Response.Success("Inventory reserved successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to reserve inventory: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response> ReleaseReservationAsync(int warehouseProductId, double quantity)
        {
            try
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts.FirstOrDefaultAsync(wp => wp.Id == warehouseProductId);
                if (warehouseProduct == null)
                    return Response.Failure("Warehouse product not found", System.Net.HttpStatusCode.NotFound);

                if (warehouseProduct.ReservedQuantity < quantity)
                    return Response.Failure("Cannot release more than reserved quantity", System.Net.HttpStatusCode.BadRequest);

                warehouseProduct.ReservedQuantity -= quantity;
                warehouseProduct.SetUpdated("system"); // TODO: Get current user

                await _unitOfWork.SaveChangesAsync();
                return Response.Success("Reservation released successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to release reservation: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<double>> GetAvailableQuantityAsync(int warehouseProductId)
        {
            try
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts.FirstOrDefaultAsync(wp => wp.Id == warehouseProductId);
                if (warehouseProduct == null)
                    return Response<double>.Failure("Warehouse product not found", System.Net.HttpStatusCode.NotFound);

                return Response<double>.Success(warehouseProduct.AvailableQuantity);
            }
            catch (Exception ex)
            {
                return Response<double>.Failure($"Failed to get available quantity: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}