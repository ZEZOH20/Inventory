using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace Inventory.DTO.Warehouse_ProductDto.Requests
{
    public class Warehouse_ProductCreateDTO
    {
        public int Product_Code { get; set; }
        public int War_Number { get; set; }
        public int Supplier_ID { get; set; }

        [JsonPropertyName("mfd")]
        public string MFD { get; set; }

        [JsonPropertyName("exp")]
        public string EXP { get; set; }
        public double Amount { get; set; } //
        public double Price { get; set; }

        public bool Valid()
        {
            return (Product_Code > 0) &&
                    (War_Number > 0) &&
                    (Supplier_ID > 0) &&
                    (Amount > 0) &&
                    (Price > 0) &&
                   (!string.IsNullOrEmpty(MFD)) &&
                   (!string.IsNullOrEmpty(EXP)); // MFD should be before EXP


        }
    }
}
