using Inventory.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.DTO.TO_ProductDto.Requests
{
    public class TO_ProductCreateDTO
    {
        public double TO_Amount { get; set; } //
        public string TO_MFD { get; set; }
        public string TO_EXP { get; set; }
        public int TO_Number { get; set; }
        public int Product_Code { get; set; }
    }
}
