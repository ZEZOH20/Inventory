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

                // Execute inventory changes based on order type
                var inventoryResult = await ExecuteInventoryChangesAsync(orderId, orderType);
                if (!inventoryResult.IsSuccess)
                    return inventoryResult;

                // Send approval notification
                await SendApprovalNotificationAsync(orderId, orderType, approverId, reviewNotes);

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

                // Release any reserved inventory
                var releaseResult = await _inventoryService.ReleaseReservationAsync(orderId, orderType);
                if (!releaseResult.IsSuccess)
                    return Response.Failure($"Failed to release inventory reservation: {releaseResult.Message}", System.Net.HttpStatusCode.InternalServerError);

                // Send rejection notification
                await SendRejectionNotificationAsync(orderId, orderType, rejectorId, reviewNotes);

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

        private async Task<Response> ExecuteInventoryChangesAsync(int orderId, OrderType orderType)
        {
            try
            {
                switch (orderType)
                {
                    case OrderType.Supply:
                        return await ExecuteSupplyOrderInventoryChangesAsync(orderId);
                    case OrderType.Release:
                        return await ExecuteReleaseOrderInventoryChangesAsync(orderId);
                    case OrderType.Transfer:
                        return await ExecuteTransferOrderInventoryChangesAsync(orderId);
                    default:
                        return Response.Failure("Unknown order type", System.Net.HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to execute inventory changes: {ex.Message}", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response> ExecuteSupplyOrderInventoryChangesAsync(int orderId)
        {
            var supplyOrder = await _unitOfWork.SupplyOrders.GetQuery()
                .Include(so => so.SO_Products)
                .FirstOrDefaultAsync(so => so.Number == orderId);

            if (supplyOrder == null)
                return Response.Failure("Supply order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in supplyOrder.SO_Products)
            {
                // Update warehouse product inventory
                var warehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == supplyOrder.War_Number && wp.Product_Code == product.Product_Code);

                if (warehouseProduct == null)
                {
                    // Create new warehouse product entry
                    warehouseProduct = new Warehouse_Product
                    {
                        War_Number = supplyOrder.War_Number,
                        Product_Code = product.Product_Code,
                        Total_Amount = product.SO_Amount,
                        ReservedQuantity = 0,
                        MFD = product.SO_MFD,
                        EXP = product.SO_EXP
                    };
                    warehouseProduct.SetCreated(supplyOrder.CreatedBy ?? "System");
                    _unitOfWork.WarehouseProducts.Add(warehouseProduct);
                }
                else
                {
                    // Update existing inventory
                    warehouseProduct.Total_Amount += product.SO_Amount;
                    warehouseProduct.SetUpdated(supplyOrder.CreatedBy ?? "System");
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Supply order inventory updated successfully");
        }

        private async Task<Response> ExecuteReleaseOrderInventoryChangesAsync(int orderId)
        {
            var releaseOrder = await _unitOfWork.ReleaseOrders.GetQuery()
                .Include(ro => ro.RO_Products)
                .FirstOrDefaultAsync(ro => ro.Number == orderId);

            if (releaseOrder == null)
                return Response.Failure("Release order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in releaseOrder.RO_Products)
            {
                // Update warehouse product inventory
                var warehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == releaseOrder.War_Number && wp.Product_Code == product.Product_Code);

                if (warehouseProduct == null)
                    return Response.Failure($"Product {product.Product_Code} not found in warehouse {releaseOrder.War_Number}", System.Net.HttpStatusCode.NotFound);

                if (warehouseProduct.AvailableQuantity < product.RO_Amount)
                    return Response.Failure($"Insufficient inventory for product {product.Product_Code}", System.Net.HttpStatusCode.BadRequest);

                warehouseProduct.Total_Amount -= product.RO_Amount;
                warehouseProduct.SetUpdated(releaseOrder.CreatedBy ?? "System");
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Release order inventory updated successfully");
        }

        private async Task<Response> ExecuteTransferOrderInventoryChangesAsync(int orderId)
        {
            var transferOrder = await _unitOfWork.TransferOrders.GetQuery()
                .Include(to => to.TO_Products)
                .FirstOrDefaultAsync(to => to.Number == orderId);

            if (transferOrder == null)
                return Response.Failure("Transfer order not found", System.Net.HttpStatusCode.NotFound);

            foreach (var product in transferOrder.TO_Products)
            {
                // Remove from source warehouse
                var sourceWarehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == transferOrder.From && wp.Product_Code == product.Product_Code);

                if (sourceWarehouseProduct == null)
                    return Response.Failure($"Product {product.Product_Code} not found in source warehouse {transferOrder.From}", System.Net.HttpStatusCode.NotFound);

                if (sourceWarehouseProduct.AvailableQuantity < product.TO_Amount)
                    return Response.Failure($"Insufficient inventory for product {product.Product_Code} in source warehouse", System.Net.HttpStatusCode.BadRequest);

                sourceWarehouseProduct.Total_Amount -= product.TO_Amount;
                sourceWarehouseProduct.SetUpdated(transferOrder.CreatedBy ?? "System");

                // Add to destination warehouse
                var destWarehouseProduct = await _unitOfWork.WarehouseProducts
                    .FirstOrDefaultAsync(wp => wp.War_Number == transferOrder.To && wp.Product_Code == product.Product_Code);

                if (destWarehouseProduct == null)
                {
                    // Create new warehouse product entry
                    destWarehouseProduct = new Warehouse_Product
                    {
                        War_Number = transferOrder.To,
                        Product_Code = product.Product_Code,
                        Total_Amount = product.TO_Amount,
                        ReservedQuantity = 0,
                        MFD = product.TO_MFD,
                        EXP = product.TO_EXP
                    };
                    destWarehouseProduct.SetCreated(transferOrder.CreatedBy ?? "System");
                    _unitOfWork.WarehouseProducts.Add(destWarehouseProduct);
                }
                else
                {
                    // Update existing inventory
                    destWarehouseProduct.Total_Amount += product.TO_Amount;
                    destWarehouseProduct.SetUpdated(transferOrder.CreatedBy ?? "System");
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Response.Success("Transfer order inventory updated successfully");
        }

        private async Task SendApprovalNotificationAsync(int orderId, OrderType orderType, string approverId, string reviewNotes)
        {
            try
            {
                var order = await GetOrderByIdAsync(orderId, orderType);
                if (order == null) return;

                var approver = await _userManager.FindByIdAsync(approverId);
                var creator = await _userManager.FindByIdAsync(order.CreatedBy ?? "");

                if (creator?.Email != null)
                {
                    var subject = $"Order {GetOrderTypePrefix(orderType)}-{orderId} Approved";
                    var body = $@"
                        <h2>Order Approval Notification</h2>
                        <p>Your {orderType.ToString().ToLower()} order has been approved.</p>
                        <p><strong>Order ID:</strong> {GetOrderTypePrefix(orderType)}-{orderId}</p>
                        <p><strong>Approved By:</strong> {approver?.UserName ?? "Unknown"}</p>
                        <p><strong>Review Notes:</strong> {reviewNotes}</p>
                        <p><strong>Approved At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    ";

                    await _emailService.SendEmailAsync(creator.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the approval process
                Console.WriteLine($"Failed to send approval notification: {ex.Message}");
            }
        }

        private async Task SendRejectionNotificationAsync(int orderId, OrderType orderType, string rejectorId, string reviewNotes)
        {
            try
            {
                var order = await GetOrderByIdAsync(orderId, orderType);
                if (order == null) return;

                var rejector = await _userManager.FindByIdAsync(rejectorId);
                var creator = await _userManager.FindByIdAsync(order.CreatedBy ?? "");

                if (creator?.Email != null)
                {
                    var subject = $"Order {GetOrderTypePrefix(orderType)}-{orderId} Rejected";
                    var body = $@"
                        <h2>Order Rejection Notification</h2>
                        <p>Your {orderType.ToString().ToLower()} order has been rejected.</p>
                        <p><strong>Order ID:</strong> {GetOrderTypePrefix(orderType)}-{orderId}</p>
                        <p><strong>Rejected By:</strong> {rejector?.UserName ?? "Unknown"}</p>
                        <p><strong>Review Notes:</strong> {reviewNotes}</p>
                        <p><strong>Rejected At:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    ";

                    await _emailService.SendEmailAsync(creator.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the rejection process
                Console.WriteLine($"Failed to send rejection notification: {ex.Message}");
            }
        }

        private string GetOrderTypePrefix(OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Supply => "SO",
                OrderType.Release => "RO",
                OrderType.Transfer => "TO",
                _ => "UNK"
            };
        }
    }
}