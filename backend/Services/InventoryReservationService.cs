using Inventory.Data.DbContexts;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Inventory.Services.CurrentUser;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class InventoryReservationService : IInventoryReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public InventoryReservationService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
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
                warehouseProduct.SetUpdated(_currentUser.UserId);

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
                warehouseProduct.SetUpdated(_currentUser.UserId);

                await _unitOfWork.SaveChangesAsync();
                return Response.Success("Reservation released successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to release reservation: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response> ReleaseReservationAsync(int orderId, OrderType orderType)
        {
            try
            {
                switch (orderType)
                {
                    case OrderType.Supply:
                        return await ReleaseSupplyOrderReservationAsync(orderId);
                    case OrderType.Release:
                        return await ReleaseReleaseOrderReservationAsync(orderId);
                    case OrderType.Transfer:
                        return await ReleaseTransferOrderReservationAsync(orderId);
                    default:
                        return Response.Failure("Unknown order type", System.Net.HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to release order reservation: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> ReleaseSupplyOrderReservationAsync(int orderId)
        {
            var supplyOrder = await _unitOfWork.SupplyOrders.GetQuery()
                .Include(so => so.SO_Products)
                .FirstOrDefaultAsync(so => so.Number == orderId);

            if (supplyOrder == null)
                return Response.Failure("Supply order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in supplyOrder.SO_Products)
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == supplyOrder.War_Number && wp.Product_Code == product.Product_Code);

                if (warehouseProduct != null && warehouseProduct.ReservedQuantity >= product.SO_Amount)
                {
                    warehouseProduct.ReservedQuantity -= product.SO_Amount;
                    warehouseProduct.SetUpdated(_currentUser.UserId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Supply order reservation released successfully");
        }

        private async Task<Response> ReleaseReleaseOrderReservationAsync(int orderId)
        {
            var releaseOrder = await _unitOfWork.ReleaseOrders.GetQuery()
                .Include(ro => ro.RO_Products)
                .FirstOrDefaultAsync(ro => ro.Number == orderId);

            if (releaseOrder == null)
                return Response.Failure("Release order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in releaseOrder.RO_Products)
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == releaseOrder.War_Number && wp.Product_Code == product.Product_Code);

                if (warehouseProduct != null && warehouseProduct.ReservedQuantity >= product.RO_Amount)
                {
                    warehouseProduct.ReservedQuantity -= product.RO_Amount;
                    warehouseProduct.SetUpdated(_currentUser.UserId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Release order reservation released successfully");
        }

        private async Task<Response> ReleaseTransferOrderReservationAsync(int orderId)
        {
            var transferOrder = await _unitOfWork.TransferOrders.GetQuery()
                .Include(to => to.TO_Products)
                .FirstOrDefaultAsync(to => to.Number == orderId);

            if (transferOrder == null)
                return Response.Failure("Transfer order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in transferOrder.TO_Products)
            {
                var warehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == transferOrder.From && wp.Product_Code == product.Product_Code);

                if (warehouseProduct != null && warehouseProduct.ReservedQuantity >= product.TO_Amount)
                {
                    warehouseProduct.ReservedQuantity -= product.TO_Amount;
                    warehouseProduct.SetUpdated(_currentUser.UserId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Transfer order reservation released successfully");
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