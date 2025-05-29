using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class SOP
    {
        [Key]
        public int Number { get; set; }
        public int Product_Code { get; set; }
        public int WarIn_Number { get; set; }

        public required string Name { get; set; }

        [Required(ErrorMessage = "Please add Manufacturing (MD) Date")]
        [DataType(DataType.Date)]
        public DateTime MFD { get; set; }

        [Required(ErrorMessage = "Please add Expire (EXP) Date")]
        [DataType(DataType.Date)]
        public DateTime EXP { get; set; }

        [DataType(DataType.Date)]
        public DateTime Supply_Date { get; set; } //

        public required int Supplier_ID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double Price { get; set; }
        public double Amount { get; set; } //
    }
}
