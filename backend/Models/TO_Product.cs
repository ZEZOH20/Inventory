using Inventory.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class TO_Product : AuditableEntity
    {
        public int Id { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double TO_Amount { get; set; } //

        [EnumDataType(typeof(WeightUnit), ErrorMessage = "Unit Doesn't Acceptable")]
        public required string TO_Unit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double TO_Price { get; set; }

        [Required(ErrorMessage = "Please add Manufacturing (MD) Date")]
        [DataType(DataType.Date)]
        public DateTime TO_MFD { get; set; }

        [Required(ErrorMessage = "Please add Expire (EXP) Date")]
        [DataType(DataType.Date)]
        public DateTime TO_EXP { get; set; }

        [ForeignKey("Transfer_Order")]
        public int TO_Number { get; set; }

        [ForeignKey("Product")]
        public int Product_Code { get; set; }


        //Navigation
        public Transfer_Order Transfer_Order { get; set; }
        public Product Product { get; set; }
    }
}
