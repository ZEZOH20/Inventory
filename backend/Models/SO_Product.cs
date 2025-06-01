using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class SO_Product
    {
        public int Id {  get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double SO_Amount { get; set; } //

        [EnumDataType(typeof(WeightUnit), ErrorMessage = "Unit Doesn't Acceptable")]
        public required string SO_Unit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double SO_Price { get; set; }

        [Required(ErrorMessage = "Please add Manufacturing (MD) Date")]
        [DataType(DataType.Date)]
        public DateTime SO_MFD { get; set; }

        [Required(ErrorMessage = "Please add Expire (EXP) Date")]
        [DataType(DataType.Date)]
        public DateTime SO_EXP { get; set; }

        [ForeignKey("Supply_Order")]
        public int SO_Number { get; set; }

        [ForeignKey("Product")]
        public int Product_Code { get; set; }


        //Navigation
        public Supply_Order Supply_Order { get; set; }
        public Product Product { get; set; }
        

    }
}
