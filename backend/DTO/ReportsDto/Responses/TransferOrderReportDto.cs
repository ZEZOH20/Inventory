using Inventory.Models;

namespace Inventory.DTO.ReportsDto.Responses
{
    public class TransferOrderReportDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string SupplierName { get; set; }
        public string FromWarehouseName { get; set; }
        public string ToWarehouseName { get; set; }
        public List<TransferOrderProductDto> Products { get; set; }
        public decimal TotalValue { get; set; }
        public string Status { get; set; }

        public TransferOrderReportDto()
        {
            Products = new List<TransferOrderProductDto>();
        }
    }

    public class TransferOrderProductDto
    {
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}