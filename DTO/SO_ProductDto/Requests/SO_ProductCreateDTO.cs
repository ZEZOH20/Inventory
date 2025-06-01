using Inventory.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.SO_ProductDto.Requests
{
    public class SO_ProductCreateDTO
    {
        public double SO_Amount { get; set; } //
        public string SO_Unit { get; set; }
        public double SO_Price { get; set; }
        public string SO_MFD { get; set; }
        public string SO_EXP { get; set; }
        public int SO_Number { get; set; }
        public int Product_Code { get; set; }

    }
}
