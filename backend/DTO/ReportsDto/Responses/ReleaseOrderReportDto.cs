using Inventory.Models;

namespace Inventory.DTO.ReportsDto.Responses
{
    public class ReleaseOrderReportDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string WarehouseName { get; set; }
        public List<ReleaseOrderProductDto> Products { get; set; }
        public decimal TotalValue { get; set; }
        public string Status { get; set; }

        public ReleaseOrderReportDto()
        {
            Products = new List<ReleaseOrderProductDto>();
        }
    }

    public class ReleaseOrderProductDto
    {
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}