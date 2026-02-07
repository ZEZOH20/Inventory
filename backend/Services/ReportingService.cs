using Inventory.Data.DbContexts;
using Inventory.DTO.ReportsDto.Requests;
using Inventory.DTO.ReportsDto.Responses;
using Inventory.Models;
using Inventory.Services.CurrentUser;
using Inventory.Shares;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CsvHelper;
using System.Globalization;

namespace Inventory.Services
{
    public class ReportingService : IReportingService
    {
        private readonly SqlDbContext _context;
        private readonly ICurrentUser _currentUser;

        public ReportingService(SqlDbContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Response<List<SupplyOrderReportDto>>> GetSupplyOrdersReportAsync(ReportRequestDto request, string userId, string userRole)
        {
            try
            {
                // Get accessible warehouse IDs based on user role
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(userId, userRole);

                var query = _context.Supply_Orders
                    .Include(so => so.Supplier)
                    .Include(so => so.Warehouse)
                    .Include(so => so.SO_Products)
                        .ThenInclude(sop => sop.Product)
                    .Where(so => so.CreatedAt >= request.StartDate && so.CreatedAt <= request.EndDate)
                    .Where(so => !request.WarehouseId.HasValue || so.War_Number == request.WarehouseId.Value)
                    .Where(so => accessibleWarehouseIds.Contains(so.War_Number))
                    .OrderByDescending(so => so.CreatedAt);

                var supplyOrders = await query.ToListAsync();

                var reportData = supplyOrders.Select(so => new SupplyOrderReportDto
                {
                    OrderId = so.Number,
                    OrderNumber = $"SO-{so.Number:D6}",
                    OrderDate = so.CreatedAt,
                    SupplierName = so.Supplier?.Name ?? "Unknown",
                    WarehouseName = so.Warehouse?.Name ?? "Unknown",
                    Status = so.Status.ToString(),
                    Products = so.SO_Products?.Select(sop => new SupplyOrderProductDto
                    {
                        ProductName = sop.Product?.Name ?? "Unknown",
                        Unit = sop.SO_Unit,
                        Quantity = (decimal)sop.SO_Amount,
                        UnitPrice = (decimal)sop.SO_Price,
                        TotalPrice = (decimal)(sop.SO_Amount * sop.SO_Price),
                        ManufacturingDate = sop.SO_MFD,
                        ExpirationDate = sop.SO_EXP
                    }).ToList() ?? new List<SupplyOrderProductDto>(),
                    TotalValue = Convert.ToDecimal(so.SO_Products?.Sum(sop => sop.SO_Amount * sop.SO_Price) ?? 0),
                }).ToList();

                return Response<List<SupplyOrderReportDto>>.Success(reportData, "Supply orders report generated successfully");
            }
            catch (Exception ex)
            {
                return Response<List<SupplyOrderReportDto>>.Failure($"Error generating supply orders report: {ex.Message}");
            }
        }

        public async Task<Response<List<ReleaseOrderReportDto>>> GetReleaseOrdersReportAsync(ReportRequestDto request, string userId, string userRole)
        {
            try
            {
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(userId, userRole);

                var query = _context.Release_Orders
                    .Include(ro => ro.Customer)
                    .Include(ro => ro.Warehouse)
                    .Include(ro => ro.RO_Products)
                        .ThenInclude(rop => rop.Product)
                    .Where(ro => ro.CreatedAt >= request.StartDate && ro.CreatedAt <= request.EndDate)
                    .Where(ro => !request.WarehouseId.HasValue || ro.War_Number == request.WarehouseId.Value)
                    .Where(ro => accessibleWarehouseIds.Contains(ro.War_Number))
                    .OrderByDescending(ro => ro.CreatedAt);

                var releaseOrders = await query.ToListAsync();

                var reportData = releaseOrders.Select(ro => new ReleaseOrderReportDto
                {
                    OrderId = ro.Number,
                    OrderNumber = $"RO-{ro.Number:D6}",
                    OrderDate = ro.CreatedAt,
                    CustomerName = ro.Customer?.Name ?? "Unknown",
                    WarehouseName = ro.Warehouse?.Name ?? "Unknown",
                    Status = ro.Status.ToString(),
                    Products = ro.RO_Products?.Select(rop => new ReleaseOrderProductDto
                    {
                        ProductName = rop.Product?.Name ?? "Unknown",
                        Unit = rop.RO_Unit,
                        Quantity = (decimal)rop.RO_Amount,
                        UnitPrice = (decimal)rop.RO_Price,
                        TotalPrice = (decimal)(rop.RO_Amount * rop.RO_Price),
                        ManufacturingDate = rop.RO_MFD,
                        ExpirationDate = rop.RO_EXP
                    }).ToList() ?? new List<ReleaseOrderProductDto>(),
                    TotalValue = Convert.ToDecimal(ro.RO_Products?.Sum(rop => rop.RO_Amount * rop.RO_Price) ?? 0),
                }).ToList();

                return Response<List<ReleaseOrderReportDto>>.Success(reportData, "Release orders report generated successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ReleaseOrderReportDto>>.Failure($"Error generating release orders report: {ex.Message}");
            }
        }

        public async Task<Response<List<TransferOrderReportDto>>> GetTransferOrdersReportAsync(ReportRequestDto request, string userId, string userRole)
        {
            try
            {
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(userId, userRole);

                var query = _context.Transfer_Orders
                    .Include(to => to.Supplier)
                    .Include(to => to.FromWarehouse)
                    .Include(to => to.ToWarehouse)
                    .Include(to => to.TO_Products)
                        .ThenInclude(top => top.Product)
                    .Where(to => to.CreatedAt >= request.StartDate && to.CreatedAt <= request.EndDate)
                    .Where(to => !request.WarehouseId.HasValue ||
                                to.From == request.WarehouseId.Value ||
                                to.To == request.WarehouseId.Value)
                    .Where(to => accessibleWarehouseIds.Contains(to.From) ||
                                accessibleWarehouseIds.Contains(to.To))
                    .OrderByDescending(to => to.CreatedAt);

                var transferOrders = await query.ToListAsync();

                var reportData = transferOrders.Select(to => new TransferOrderReportDto
                {
                    OrderId = to.Number,
                    OrderNumber = $"TO-{to.Number:D6}",
                    OrderDate = to.CreatedAt,
                    SupplierName = to.Supplier?.Name ?? "Unknown",
                    FromWarehouseName = to.FromWarehouse?.Name ?? "Unknown",
                    ToWarehouseName = to.ToWarehouse?.Name ?? "Unknown",
                    Status = to.Status.ToString(),
                    Products = to.TO_Products?.Select(top => new TransferOrderProductDto
                    {
                        ProductName = top.Product?.Name ?? "Unknown",
                        Unit = top.TO_Unit,
                        Quantity = (decimal)top.TO_Amount,
                        UnitPrice = (decimal)top.TO_Price,
                        TotalPrice = (decimal)(top.TO_Amount * top.TO_Price),
                        ManufacturingDate = top.TO_MFD,
                        ExpirationDate = top.TO_EXP
                    }).ToList() ?? new List<TransferOrderProductDto>(),
                    TotalValue = Convert.ToDecimal(to.TO_Products?.Sum(top => top.TO_Amount * top.TO_Price) ?? 0),
                }).ToList();

                return Response<List<TransferOrderReportDto>>.Success(reportData, "Transfer orders report generated successfully");
            }
            catch (Exception ex)
            {
                return Response<List<TransferOrderReportDto>>.Failure($"Error generating transfer orders report: {ex.Message}");
            }
        }

        public async Task<Response<FinancialSummaryDto>> GetFinancialSummaryReportAsync(ReportRequestDto request, string userId, string userRole)
        {
            try
            {
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(userId, userRole);

                var financialSummary = new FinancialSummaryDto();

                // Calculate supply costs
                var supplyCosts = await _context.SO_Products
                    .Include(sop => sop.Supply_Order)
                    .Where(sop => sop.Supply_Order.CreatedAt >= request.StartDate && sop.Supply_Order.CreatedAt <= request.EndDate)
                    .Where(sop => !request.WarehouseId.HasValue || sop.Supply_Order.War_Number == request.WarehouseId.Value)
                    .Where(sop => accessibleWarehouseIds.Contains(sop.Supply_Order.War_Number))
                    .GroupBy(sop => sop.Supply_Order.Warehouse.Name)
                    .Select(g => new { WarehouseName = g.Key, TotalCost = (decimal)g.Sum(sop => sop.SO_Amount * sop.SO_Price) })
                    .ToListAsync();

                financialSummary.TotalSupplyCosts = (decimal)supplyCosts.Sum(x => x.TotalCost);
                financialSummary.TotalSupplyOrders = await _context.Supply_Orders
                    .Where(so => so.CreatedAt >= request.StartDate && so.CreatedAt <= request.EndDate)
                    .Where(so => !request.WarehouseId.HasValue || so.War_Number == request.WarehouseId.Value)
                    .Where(so => accessibleWarehouseIds.Contains(so.War_Number))
                    .CountAsync();

                foreach (var cost in supplyCosts)
                {
                    financialSummary.WarehouseCosts[cost.WarehouseName] = cost.TotalCost;
                }

                // Calculate release revenues
                var releaseRevenues = await _context.RO_Product
                    .Include(rop => rop.Release_Order)
                    .Where(rop => rop.Release_Order.CreatedAt >= request.StartDate && rop.Release_Order.CreatedAt <= request.EndDate)
                    .Where(rop => !request.WarehouseId.HasValue || rop.Release_Order.War_Number == request.WarehouseId.Value)
                    .Where(rop => accessibleWarehouseIds.Contains(rop.Release_Order.War_Number))
                    .GroupBy(rop => rop.Release_Order.Warehouse.Name)
                    .Select(g => new { WarehouseName = g.Key, TotalRevenue = (decimal)g.Sum(rop => rop.RO_Amount * rop.RO_Price) })
                    .ToListAsync();

                financialSummary.TotalReleaseRevenues = (decimal)releaseRevenues.Sum(x => x.TotalRevenue);
                financialSummary.TotalReleaseOrders = await _context.Release_Orders
                    .Where(ro => ro.CreatedAt >= request.StartDate && ro.CreatedAt <= request.EndDate)
                    .Where(ro => !request.WarehouseId.HasValue || ro.War_Number == request.WarehouseId.Value)
                    .Where(ro => accessibleWarehouseIds.Contains(ro.War_Number))
                    .CountAsync();

                foreach (var revenue in releaseRevenues)
                {
                    financialSummary.WarehouseRevenues[revenue.WarehouseName] = revenue.TotalRevenue;
                }

                // Calculate transfer costs (transfers are considered costs)
                var transferCosts = await _context.TO_Products
                    .Include(top => top.Transfer_Order)
                    .Where(top => top.Transfer_Order.CreatedAt >= request.StartDate && top.Transfer_Order.CreatedAt <= request.EndDate)
                    .Where(top => !request.WarehouseId.HasValue ||
                                 top.Transfer_Order.From == request.WarehouseId.Value ||
                                 top.Transfer_Order.To == request.WarehouseId.Value)
                    .Where(top => accessibleWarehouseIds.Contains(top.Transfer_Order.From) ||
                                 accessibleWarehouseIds.Contains(top.Transfer_Order.To))
                    .SumAsync(top => top.TO_Amount * top.TO_Price);

                financialSummary.TotalTransferCosts = Convert.ToDecimal(transferCosts);
                financialSummary.TotalTransferOrders = await _context.Transfer_Orders
                    .Where(to => to.CreatedAt >= request.StartDate && to.CreatedAt <= request.EndDate)
                    .Where(to => !request.WarehouseId.HasValue ||
                                to.From == request.WarehouseId.Value ||
                                to.To == request.WarehouseId.Value)
                    .Where(to => accessibleWarehouseIds.Contains(to.From) ||
                                accessibleWarehouseIds.Contains(to.To))
                    .CountAsync();

                // Calculate net profit/loss
                financialSummary.NetProfitLoss = financialSummary.TotalReleaseRevenues - (financialSummary.TotalSupplyCosts + financialSummary.TotalTransferCosts);

                return Response<FinancialSummaryDto>.Success(financialSummary, "Financial summary report generated successfully");
            }
            catch (Exception ex)
            {
                return Response<FinancialSummaryDto>.Failure($"Error generating financial summary report: {ex.Message}");
            }
        }

        private async Task<List<int>> GetAccessibleWarehouseIdsAsync(string userId, string userRole)
        {
            Console.WriteLine($"GetAccessibleWarehouseIdsAsync called with UserId: {userId}, UserRole: {userRole}");

            if (userRole == "Owner")
            {
                // Owners can access warehouses they created
                var warehouses = await _context.Warehouses.Where(w => w.CreatedBy == userId && !w.IsDeleted).Select(w => w.Number).ToListAsync();
                Console.WriteLine($"Warehouses found for owner {userId}: {warehouses.Count}");
                return warehouses;
            }
            else if (userRole == "Manager")
            {
                // Managers can only access their assigned warehouse
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                var warehouseIds = user?.WarehouseId.HasValue == true ? new List<int> { user.WarehouseId.Value } : new List<int>();
                Console.WriteLine($"Warehouse found for manager {userId}: {(warehouseIds.Count > 0 ? warehouseIds[0].ToString() : "None")}");
                return warehouseIds;
            }
            else
            {
                // Employees have no warehouse access for reports
                Console.WriteLine($"User {userId} with role {userRole} has no warehouse access for reports");
                return new List<int>();
            }
        }

        public async Task<byte[]> ExportSupplyOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole)
        {
            var reportResponse = await GetSupplyOrdersReportAsync(request, userId, userRole);
            if (!reportResponse.IsSuccess)
            {
                throw new Exception($"Failed to generate supply orders report: {reportResponse.Message}");
            }

            var data = reportResponse.Data ?? new List<SupplyOrderReportDto>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Supply Orders Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            foreach (var order in data)
                            {
                                column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(orderColumn =>
                                {
                                    orderColumn.Item().Text($"Order Number: {order.OrderNumber ?? "N/A"}").Bold();
                                    orderColumn.Item().Text($"Date: {order.OrderDate:yyyy-MM-dd}");
                                    orderColumn.Item().Text($"Supplier: {order.SupplierName ?? "N/A"}");
                                    orderColumn.Item().Text($"Warehouse: {order.WarehouseName ?? "N/A"}");
                                    orderColumn.Item().Text($"Status: {order.Status ?? "N/A"}");
                                    orderColumn.Item().Text($"Total Value: {order.TotalValue:C}").Bold();

                                    if (order.Products != null && order.Products.Any())
                                    {
                                        orderColumn.Item().PaddingTop(10).Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Product").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Unit").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Qty").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Unit Price").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Total").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("MFD").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("EXP").Bold().FontSize(11);
                                            });

                                            foreach (var product in order.Products)
                                            {
                                                var rowIndex = Array.IndexOf(order.Products.ToArray(), product);
                                                var backgroundColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ProductName ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.Unit ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.Quantity.ToString("N2") ?? "0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.UnitPrice.ToString("C") ?? "$0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.TotalPrice.ToString("C") ?? "$0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(10);
                                            }
                                        });
                                    }
                                });
                                column.Item().PaddingBottom(1, QuestPDF.Infrastructure.Unit.Centimetre);
                            }

                            if (!data.Any())
                            {
                                column.Item().Text("No supply orders found for the selected period.").Italic();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportReleaseOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole)
        {
            var reportResponse = await GetReleaseOrdersReportAsync(request, userId, userRole);
            if (!reportResponse.IsSuccess)
            {
                throw new Exception($"Failed to generate release orders report: {reportResponse.Message}");
            }

            var data = reportResponse.Data ?? new List<ReleaseOrderReportDto>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Release Orders Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            foreach (var order in data)
                            {
                                column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(orderColumn =>
                                {
                                    orderColumn.Item().Text($"Order Number: {order.OrderNumber ?? "N/A"}").Bold();
                                    orderColumn.Item().Text($"Date: {order.OrderDate:yyyy-MM-dd}");
                                    orderColumn.Item().Text($"Customer: {order.CustomerName ?? "N/A"}");
                                    orderColumn.Item().Text($"Warehouse: {order.WarehouseName ?? "N/A"}");
                                    orderColumn.Item().Text($"Status: {order.Status ?? "N/A"}");
                                    orderColumn.Item().Text($"Total Value: {order.TotalValue:C}").Bold();

                                    if (order.Products != null && order.Products.Any())
                                    {
                                        orderColumn.Item().PaddingTop(10).Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Product").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Unit").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Qty").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Unit Price").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("Total").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("MFD").Bold().FontSize(11);
                                                header.Cell().Border(1).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).Padding(8).Text("EXP").Bold().FontSize(11);
                                            });

                                            foreach (var product in order.Products)
                                            {
                                                var rowIndex = Array.IndexOf(order.Products.ToArray(), product);
                                                var backgroundColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ProductName ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.Unit ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.Quantity.ToString("N2") ?? "0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.UnitPrice.ToString("C") ?? "$0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.TotalPrice.ToString("C") ?? "$0.00").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(10);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten1).Background(backgroundColor).Padding(6).Text(product?.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(10);
                                            }
                                        });
                                    }
                                });
                                column.Item().PaddingBottom(1, QuestPDF.Infrastructure.Unit.Centimetre);
                            }

                            if (!data.Any())
                            {
                                column.Item().Text("No release orders found for the selected period.").Italic();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportTransferOrdersReportPdfAsync(ReportRequestDto request, string userId, string userRole)
        {
            var reportResponse = await GetTransferOrdersReportAsync(request, userId, userRole);
            if (!reportResponse.IsSuccess)
            {
                throw new Exception($"Failed to generate transfer orders report: {reportResponse.Message}");
            }

            var data = reportResponse.Data ?? new List<TransferOrderReportDto>();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Transfer Orders Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            foreach (var order in data)
                            {
                                column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(orderColumn =>
                                {
                                    orderColumn.Item().Text($"Order Number: {order.OrderNumber ?? "N/A"}").Bold();
                                    orderColumn.Item().Text($"Date: {order.OrderDate:yyyy-MM-dd}");
                                    orderColumn.Item().Text($"Supplier: {order.SupplierName ?? "N/A"}");
                                    orderColumn.Item().Text($"From Warehouse: {order.FromWarehouseName ?? "N/A"}");
                                    orderColumn.Item().Text($"To Warehouse: {order.ToWarehouseName ?? "N/A"}");
                                    orderColumn.Item().Text($"Status: {order.Status ?? "N/A"}");
                                    orderColumn.Item().Text($"Total Value: {order.TotalValue:C}").Bold();

                                    if (order.Products != null && order.Products.Any())
                                    {
                                        orderColumn.Item().PaddingTop(10).Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1.5f);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                            });

                                            // Add table header
                                            table.Header(header =>
                                            {
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Product Name").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Unit").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Quantity").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Unit Price").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Total Price").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Mfg Date").Bold().FontSize(10);
                                                header.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(Colors.Grey.Lighten3).Padding(5).Text("Exp Date").Bold().FontSize(10);
                                            });

                                            int rowIndex = 0;
                                            foreach (var product in order.Products)
                                            {
                                                var backgroundColor = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.ProductName ?? "N/A").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.Unit ?? "N/A").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.Quantity.ToString("N2") ?? "0.00").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.UnitPrice.ToString("C") ?? "$0.00").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.TotalPrice.ToString("C") ?? "$0.00").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(9);
                                                table.Cell().Border(1).BorderColor(Colors.Grey.Medium).Background(backgroundColor).Padding(5).Text(product?.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A").FontSize(9);
                                                rowIndex++;
                                            }
                                        });
                                    }
                                });
                                column.Item().PaddingBottom(1, QuestPDF.Infrastructure.Unit.Centimetre);
                            }

                            if (!data.Any())
                            {
                                column.Item().Text("No transfer orders found for the selected period.").Italic();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportFinancialSummaryReportPdfAsync(ReportRequestDto request, string userId, string userRole)
        {
            var reportResponse = await GetFinancialSummaryReportAsync(request, userId, userRole);
            if (!reportResponse.IsSuccess)
            {
                throw new Exception($"Failed to generate financial summary report: {reportResponse.Message}");
            }

            var data = reportResponse.Data;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Financial Summary Report")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Summary Section
                            column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(summaryColumn =>
                            {
                                summaryColumn.Item().Text("Financial Summary").Bold().FontSize(16);
                                summaryColumn.Item().PaddingTop(10).Text($"Total Supply Costs: {(data?.TotalSupplyCosts ?? 0):C}");
                                summaryColumn.Item().Text($"Total Release Revenues: {(data?.TotalReleaseRevenues ?? 0):C}");
                                summaryColumn.Item().Text($"Total Transfer Costs: {(data?.TotalTransferCosts ?? 0):C}");
                                summaryColumn.Item().Text($"Net Profit/Loss: {(data?.NetProfitLoss ?? 0):C}").Bold();
                                summaryColumn.Item().PaddingTop(10).Text("Order Counts").Bold();
                                summaryColumn.Item().Text($"Total Supply Orders: {data?.TotalSupplyOrders ?? 0}");
                                summaryColumn.Item().Text($"Total Release Orders: {data?.TotalReleaseOrders ?? 0}");
                                summaryColumn.Item().Text($"Total Transfer Orders: {data?.TotalTransferOrders ?? 0}");
                            });

                            // Warehouse Costs
                            if (data?.WarehouseCosts != null && data.WarehouseCosts.Any())
                            {
                                column.Item().PaddingTop(20).Text("Warehouse Costs").Bold().FontSize(16);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).Text("Warehouse").SemiBold();
                                        header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).Text("Cost").SemiBold();
                                    });

                                    foreach (var cost in data.WarehouseCosts)
                                    {
                                        table.Cell().PaddingVertical(5).Text(cost.Key ?? "N/A");
                                        table.Cell().PaddingVertical(5).Text(cost.Value.ToString("C"));
                                    }
                                });
                            }

                            // Warehouse Revenues
                            if (data?.WarehouseRevenues != null && data.WarehouseRevenues.Any())
                            {
                                column.Item().PaddingTop(20).Text("Warehouse Revenues").Bold().FontSize(16);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).Text("Warehouse").SemiBold();
                                        header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(5).Text("Revenue").SemiBold();
                                    });

                                    foreach (var revenue in data.WarehouseRevenues)
                                    {
                                        table.Cell().PaddingVertical(5).Text(revenue.Key ?? "N/A");
                                        table.Cell().PaddingVertical(5).Text(revenue.Value.ToString("C"));
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        // CSV Export Methods
        public async Task<string> ExportSupplyOrdersReportCsvAsync(ReportRequestDto request, string userId, string userRole)
        {
            var response = await GetSupplyOrdersReportAsync(request, userId, userRole);
            if (!response.IsSuccess || response.Data == null)
            {
                return "Error: Unable to retrieve supply orders data";
            }

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write header
            csv.WriteField("Order Number");
            csv.WriteField("Order Date");
            csv.WriteField("Supplier Name");
            csv.WriteField("Warehouse Name");
            csv.WriteField("Total Value");
            csv.WriteField("Product Name");
            csv.WriteField("Unit");
            csv.WriteField("Quantity");
            csv.WriteField("Unit Price");
            csv.WriteField("Total Price");
            csv.WriteField("Manufacturing Date");
            csv.WriteField("Expiration Date");
            await csv.NextRecordAsync();

            // Write data
            foreach (var order in response.Data)
            {
                if (order.Products != null && order.Products.Any())
                {
                    foreach (var product in order.Products)
                    {
                        csv.WriteField(order.OrderNumber);
                        csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                        csv.WriteField(order.SupplierName);
                        csv.WriteField(order.WarehouseName);
                        csv.WriteField(order.TotalValue.ToString("F2"));
                        csv.WriteField(product.ProductName ?? "N/A");
                        csv.WriteField(product.Unit ?? "N/A");
                        csv.WriteField(product.Quantity.ToString("F2"));
                        csv.WriteField(product.UnitPrice.ToString("F2"));
                        csv.WriteField(product.TotalPrice.ToString("F2"));
                        csv.WriteField(product.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        csv.WriteField(product.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        await csv.NextRecordAsync();
                    }
                }
                else
                {
                    // Order with no products
                    csv.WriteField(order.OrderNumber);
                    csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                    csv.WriteField(order.SupplierName);
                    csv.WriteField(order.WarehouseName);
                    csv.WriteField(order.TotalValue.ToString("F2"));
                    csv.WriteField("No products");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    await csv.NextRecordAsync();
                }
            }

            return writer.ToString();
        }

        public async Task<string> ExportReleaseOrdersReportCsvAsync(ReportRequestDto request, string userId, string userRole)
        {
            var response = await GetReleaseOrdersReportAsync(request, userId, userRole);
            if (!response.IsSuccess || response.Data == null)
            {
                return "Error: Unable to retrieve release orders data";
            }

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write header
            csv.WriteField("Order Number");
            csv.WriteField("Order Date");
            csv.WriteField("Customer Name");
            csv.WriteField("Warehouse Name");
            csv.WriteField("Total Value");
            csv.WriteField("Product Name");
            csv.WriteField("Unit");
            csv.WriteField("Quantity");
            csv.WriteField("Unit Price");
            csv.WriteField("Total Price");
            csv.WriteField("Manufacturing Date");
            csv.WriteField("Expiration Date");
            await csv.NextRecordAsync();

            // Write data
            foreach (var order in response.Data)
            {
                if (order.Products != null && order.Products.Any())
                {
                    foreach (var product in order.Products)
                    {
                        csv.WriteField(order.OrderNumber);
                        csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                        csv.WriteField(order.CustomerName);
                        csv.WriteField(order.WarehouseName);
                        csv.WriteField(order.TotalValue.ToString("F2"));
                        csv.WriteField(product.ProductName ?? "N/A");
                        csv.WriteField(product.Unit ?? "N/A");
                        csv.WriteField(product.Quantity.ToString("F2"));
                        csv.WriteField(product.UnitPrice.ToString("F2"));
                        csv.WriteField(product.TotalPrice.ToString("F2"));
                        csv.WriteField(product.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        csv.WriteField(product.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        await csv.NextRecordAsync();
                    }
                }
                else
                {
                    // Order with no products
                    csv.WriteField(order.OrderNumber);
                    csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                    csv.WriteField(order.CustomerName);
                    csv.WriteField(order.WarehouseName);
                    csv.WriteField(order.TotalValue.ToString("F2"));
                    csv.WriteField("No products");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    await csv.NextRecordAsync();
                }
            }

            return writer.ToString();
        }

        public async Task<string> ExportTransferOrdersReportCsvAsync(ReportRequestDto request, string userId, string userRole)
        {
            var response = await GetTransferOrdersReportAsync(request, userId, userRole);
            if (!response.IsSuccess || response.Data == null)
            {
                return "Error: Unable to retrieve transfer orders data";
            }

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write header
            csv.WriteField("Order Number");
            csv.WriteField("Order Date");
            csv.WriteField("Supplier Name");
            csv.WriteField("From Warehouse");
            csv.WriteField("To Warehouse");
            csv.WriteField("Total Value");
            csv.WriteField("Product Name");
            csv.WriteField("Unit");
            csv.WriteField("Quantity");
            csv.WriteField("Unit Price");
            csv.WriteField("Total Price");
            csv.WriteField("Manufacturing Date");
            csv.WriteField("Expiration Date");
            await csv.NextRecordAsync();

            // Write data
            foreach (var order in response.Data)
            {
                if (order.Products != null && order.Products.Any())
                {
                    foreach (var product in order.Products)
                    {
                        csv.WriteField(order.OrderNumber);
                        csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                        csv.WriteField(order.SupplierName);
                        csv.WriteField(order.FromWarehouseName);
                        csv.WriteField(order.ToWarehouseName);
                        csv.WriteField(order.TotalValue.ToString("F2"));
                        csv.WriteField(product.ProductName ?? "N/A");
                        csv.WriteField(product.Unit ?? "N/A");
                        csv.WriteField(product.Quantity.ToString("F2"));
                        csv.WriteField(product.UnitPrice.ToString("F2"));
                        csv.WriteField(product.TotalPrice.ToString("F2"));
                        csv.WriteField(product.ManufacturingDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        csv.WriteField(product.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A");
                        await csv.NextRecordAsync();
                    }
                }
                else
                {
                    // Order with no products
                    csv.WriteField(order.OrderNumber);
                    csv.WriteField(order.OrderDate.ToString("yyyy-MM-dd"));
                    csv.WriteField(order.SupplierName);
                    csv.WriteField(order.FromWarehouseName);
                    csv.WriteField(order.ToWarehouseName);
                    csv.WriteField(order.TotalValue.ToString("F2"));
                    csv.WriteField("No products");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    csv.WriteField("");
                    await csv.NextRecordAsync();
                }
            }

            return writer.ToString();
        }
    }
}