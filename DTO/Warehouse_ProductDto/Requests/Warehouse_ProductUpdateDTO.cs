using Microsoft.IdentityModel.Tokens;
using System.Numerics;

namespace Inventory.DTO.Warehouse_ProductDto.Requests
{
    public class Warehouse_ProductUpdateDTO
    {
        public int Id { get; set; }
        public int? Product_Code { get; set; }
        public int? War_Number { get; set; }
        public int? Supplier_ID { get; set; }
        public DateTime? MFD { get; set; }
        public DateTime? EXP { get; set; }
        public DateTime? Store_Date { get; set; }
        public double? Total_Amount { get; set; }
        public double? Total_Price { get; set; }
        // Custom validation to ensure at least one field is provided
        public bool HasAtLeastOneValue()
        {
            return !(Product_Code < 0) ||
                    !(War_Number < 0) ||
                    !(Supplier_ID < 0) ||
                    !(Total_Amount < 0) ||
                    !(Total_Price < 0) ||
                     MFD !=null||
                     EXP != null||
                     Store_Date != null;
        }
    }
}
