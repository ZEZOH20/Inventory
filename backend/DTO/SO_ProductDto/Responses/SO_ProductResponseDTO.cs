using Inventory.DTO.ProductDto.Responses;
using Inventory.Shares;
using System;

namespace Inventory.DTO.SO_ProductDto.Responses
{
    public class SO_ProductResponseDTO
    {
        public int Id { get; set; }
        public double SO_Amount { get; set; }
        public string SO_Unit { get; set; }
        public double SO_Price { get; set; }
        public DateTime SO_MFD { get; set; }
        public DateTime SO_EXP { get; set; }
        public int SO_Number { get; set; }
        public int Product_Code { get; set; }
        public ProductResponseDTO Product { get; set; }
        public string SupplierName { get; set; }
        public string WarehouseName { get; set; }
        public DateTime S_Date { get; set; }
        public OrderStatus Status { get; set; }
    }
}