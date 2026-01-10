using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Inventory.Models
{
    public class Warehouse_Product : AuditableEntity
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int Product_Code { get; set; }

        [ForeignKey("Warehouse")]
        public int War_Number { get; set; }

        [ForeignKey("Supplier")]
        public int Supplier_ID { get; set; }

        [Required(ErrorMessage = "Please add Manufacturing (MD) Date")]
        [DataType(DataType.Date)]
        public DateTime MFD { get; set; }

        [Required(ErrorMessage = "Please add Expire (EXP) Date")]
        [DataType(DataType.Date)]
        public DateTime EXP { get; set; }

        [DataType(DataType.Date)]
        public DateTime Store_Date { get; set; } //

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double Total_Amount { get; set; } //

        [Range(0, double.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public double Total_Price { get; set; }


        //Navigation
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
        public Supplier Supplier { get; set; }

    }
}
