using Inventory.Data.DbContexts;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Inventory.Services.Auth;
using Inventory.Services.CurrentUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryReservationService _inventoryService;
        private readonly ISendEmailService _emailService;
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SqlDbContext _context;

        public ApprovalService(
            IUnitOfWork unitOfWork,
            IInventoryReservationService inventoryService,
            ISendEmailService emailService,
            ICurrentUser currentUser,
            UserManager<ApplicationUser> userManager,
            SqlDbContext context)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _emailService = emailService;
            _currentUser = currentUser;
            _userManager = userManager;
            _context = context;
        }

        public async Task<Response> ApproveOrderAsync(int orderId, OrderType orderType, string reviewNotes, string approverId)
        {
            try
            {
                // Validate approver permissions
                var canApprove = await CanUserApproveOrderAsync(approverId, orderId, orderType);
                if (!canApprove.IsSuccess)
                    return canApprove;

                // Get and validate order
                var orderEntity = await GetOrderByIdAsync(orderId, orderType);
                if (orderEntity == null)
                    return Response.Failure("Order not found", System.Net.HttpStatusCode.NotFound);

                // Cast to access order-specific properties
                dynamic order = orderEntity;
                if (order.Status != OrderStatus.Pending)
                    return Response.Failure("Order is not in pending status", System.Net.HttpStatusCode.BadRequest);

                // Update order status
                order.Status = OrderStatus.Approved;
                order.ApprovedBy = approverId;
                order.ApprovedAt = DateTime.UtcNow;
                order.ReviewNotes = reviewNotes;
                order.SetUpdated(approverId);

                await _unitOfWork.SaveChangesAsync();

                // TODO: Execute inventory changes based on order type
                // TODO: Send approval notification

                return Response.Success("Order approved successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to approve order: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response> RejectOrderAsync(int orderId, OrderType orderType, string reviewNotes, string rejectorId)
        {
            try
            {
                // Validate rejector permissions
                var canReject = await CanUserApproveOrderAsync(rejectorId, orderId, orderType);
                if (!canReject.IsSuccess)
                    return canReject;

                // Get and validate order
                var orderEntity = await GetOrderByIdAsync(orderId, orderType);
                if (orderEntity == null)
                    return Response.Failure("Order not found", System.Net.HttpStatusCode.NotFound);

                // Cast to access order-specific properties
                dynamic order = orderEntity;
                if (order.Status != OrderStatus.Pending)
                    return Response.Failure("Order is not in pending status", System.Net.HttpStatusCode.BadRequest);

                // Update order status
                order.Status = OrderStatus.Rejected;
                order.ApprovedBy = rejectorId;
                order.ApprovedAt = DateTime.UtcNow;
                order.ReviewNotes = reviewNotes;
                order.SetUpdated(rejectorId);

                await _unitOfWork.SaveChangesAsync();

                // TODO: Release any reserved inventory
                // TODO: Send rejection notification

                return Response.Success("Order rejected successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to reject order: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<List<PendingOrderDto>>> GetPendingOrdersAsync(string userId, int? warehouseId = null)
        {
            try
            {
                var pendingOrders = new List<PendingOrderDto>();

                // Get user roles
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Response<List<PendingOrderDto>>.Failure("User not found", System.Net.HttpStatusCode.NotFound);

                var userRoles = await _userManager.GetRolesAsync(user);
                bool isOwner = userRoles.Contains("Owner");
                bool isManager = userRoles.Contains("Manager");

                int? managerWarehouseId = null;
                if (isManager && !isOwner)
                {
                    // Get manager's warehouse
                    var managerWarehouse = await _unitOfWork.Warehouses
                        .FirstOrDefaultAsync(w => w.ManagerId == userId);
                    managerWarehouseId = managerWarehouse?.Number;
                }

                // Get pending supply orders
                IQueryable<Supply_Order> supplyOrdersQuery = _unitOfWork.SupplyOrders.GetQuery()
                    .Where(so => so.Status == OrderStatus.Pending)
                    .Include(so => so.Supplier)
                    .Include(so => so.Warehouse);

                if (!isOwner && managerWarehouseId.HasValue)
                {
                    supplyOrdersQuery = supplyOrdersQuery.Where(so => so.War_Number == managerWarehouseId.Value);
                }

                var supplyOrders = await supplyOrdersQuery.ToListAsync();

                foreach (var order in supplyOrders)
                {
                    pendingOrders.Add(new PendingOrderDto
                    {
                        OrderId = order.Number,
                        OrderType = OrderType.Supply,
                        OrderNumber = $"SO-{order.Number}",
                        SubmittedAt = order.CreatedAt,
                        SubmittedBy = order.CreatedBy ?? "Unknown",
                        WarehouseId = order.War_Number,
                        WarehouseName = order.Warehouse?.Name ?? "Unknown",
                        TotalValue = 0, // TODO: Calculate from products
                        Status = order.Status.ToString()
                    });
                }

                // Similar logic for release and transfer orders would go here

                return Response<List<PendingOrderDto>>.Success(pendingOrders);
            }
            catch (Exception ex)
            {
                return Response<List<PendingOrderDto>>.Failure($"Failed to get pending orders: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response> CancelOrderAsync(int orderId, OrderType orderType, string cancellationReason, string userId)
        {
            try
            {
                // Get and validate order
                var orderEntity = await GetOrderByIdAsync(orderId, orderType);
                if (orderEntity == null)
                    return Response.Failure("Order not found", System.Net.HttpStatusCode.NotFound);

                // Cast to access order-specific properties
                dynamic order = orderEntity;
                if (order.Status != OrderStatus.Pending)
                    return Response.Failure("Only pending orders can be cancelled", System.Net.HttpStatusCode.BadRequest);

                if (order.CreatedBy != userId)
                    return Response.Failure("Only the creator can cancel the order", System.Net.HttpStatusCode.Forbidden);

                // Update order status
                order.Status = OrderStatus.Cancelled;
                order.CancellationReason = cancellationReason;
                order.SetUpdated(userId);

                await _unitOfWork.SaveChangesAsync();

                return Response.Success("Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to cancel order: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> CanUserApproveOrderAsync(string userId, int orderId, OrderType orderType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Response.Failure("User not found", System.Net.HttpStatusCode.NotFound);

            var userRoles = await _userManager.GetRolesAsync(user);
            bool isOwner = userRoles.Contains("Owner");
            bool isManager = userRoles.Contains("Manager");

            if (!isOwner && !isManager)
                return Response.Failure("User does not have approval permissions", System.Net.HttpStatusCode.Forbidden);

            if (isManager && !isOwner)
            {
                // Check if manager manages the warehouse for this order
                var warehouseId = await GetWarehouseIdForOrderAsync(orderId, orderType);
                var managerWarehouse = await _unitOfWork.Warehouses
                    .FirstOrDefaultAsync(w => w.ManagerId == userId);

                if (managerWarehouse?.Number != warehouseId)
                    return Response.Failure("Manager can only approve orders for their warehouse", System.Net.HttpStatusCode.Forbidden);
            }

            return Response.Success();
        }

        private async Task<int?> GetWarehouseIdForOrderAsync(int orderId, OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Supply => (await _unitOfWork.SupplyOrders.FirstOrDefaultAsync(so => so.Number == orderId))?.War_Number,
                OrderType.Release => (await _unitOfWork.ReleaseOrders.FirstOrDefaultAsync(ro => ro.Number == orderId))?.War_Number,
                OrderType.Transfer => (await _unitOfWork.TransferOrders.FirstOrDefaultAsync(to => to.Number == orderId))?.From,
                _ => null
            };
        }

        private async Task<AuditableEntity?> GetOrderByIdAsync(int orderId, OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Supply => await _unitOfWork.SupplyOrders.FirstOrDefaultAsync(so => so.Number == orderId),
                OrderType.Release => await _unitOfWork.ReleaseOrders.FirstOrDefaultAsync(ro => ro.Number == orderId),
                OrderType.Transfer => await _unitOfWork.TransferOrders.FirstOrDefaultAsync(to => to.Number == orderId),
                _ => null
            };
        }
    }
}