namespace Inventory.DTO.ReportsDto.Responses
{
    public class FinancialSummaryDto
    {
        public decimal TotalSupplyCosts { get; set; }
        public decimal TotalReleaseRevenues { get; set; }
        public decimal TotalTransferCosts { get; set; }
        public decimal NetProfitLoss { get; set; }
        public int TotalSupplyOrders { get; set; }
        public int TotalReleaseOrders { get; set; }
        public int TotalTransferOrders { get; set; }
        public Dictionary<string, decimal> WarehouseCosts { get; set; }
        public Dictionary<string, decimal> WarehouseRevenues { get; set; }

        public FinancialSummaryDto()
        {
            WarehouseCosts = new Dictionary<string, decimal>();
            WarehouseRevenues = new Dictionary<string, decimal>();
        }
    }
}