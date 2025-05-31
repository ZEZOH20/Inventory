using Inventory.DTO.ProductDto.Responses;

namespace Inventory.DTO.Warehouse_ProductDto.Responses
{
    public class Warehouse_ProductResponseDTO
    {
        public int Id { get; set; } 
        public int Product_Code { get; set; }
        public int War_Number { get; set; }
        public int Supplier_ID { get; set; }
        public DateTime MFD { get; set; }
        public DateTime EXP { get; set; }
        public DateTime Store_Date { get; set; } //
        public double Total_Amount { get; set; } //
        public double Total_Price { get; set; }
        public ProductResponseDTO Product { get; set; }

        public string SupplierName { get; set; }
    }
}
