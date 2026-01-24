using Inventory.DTO.ProductDto.Responses;
using Inventory.Shares;
using System;

namespace backend.DTO.TO_ProductDto.Responses
{
    public class TO_ProductResponseDTO
    {
        public int Id { get; set; }
        public double TO_Amount { get; set; }
        public string TO_Unit { get; set; }
        public double TO_Price { get; set; }
        public DateTime TO_MFD { get; set; }
        public DateTime TO_EXP { get; set; }
        public int TO_Number { get; set; }
        public int Product_Code { get; set; }
        public ProductResponseDTO Product { get; set; }
        public string SupplierName { get; set; }
        public string FromWarehouseName { get; set; }
        public string ToWarehouseName { get; set; }
        public DateTime T_Date { get; set; }
        public OrderStatus Status { get; set; }
    }
}