using Inventory.DTO.ProductDto.Responses;
using Inventory.Shares;
using System;

namespace Inventory.DTO.RO_ProductDto.Responses
{
    public class RO_ProductResponseDTO
    {
        public int Id { get; set; }
        public double RO_Amount { get; set; }
        public string RO_Unit { get; set; }
        public double RO_Price { get; set; }
        public DateTime RO_MFD { get; set; }
        public DateTime RO_EXP { get; set; }
        public int RO_Number { get; set; }
        public int Product_Code { get; set; }
        public ProductResponseDTO Product { get; set; }
        public string CustomerName { get; set; }
        public string WarehouseName { get; set; }
        public DateTime R_Date { get; set; }
        public OrderStatus Status { get; set; }
    }
}