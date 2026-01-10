using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Supply_Order : AuditableEntity
    {
        [Key]
        public int Number { get; set; }

        [ForeignKey("Supplier")]
        public required int Supplier_ID { get; set; }

        [ForeignKey("Warehouse")]
        public required int War_Number { get; set; }

        [DataType(DataType.Date)]
        public DateTime S_Date { get; set; }

        //Navigation
        public Supplier Supplier { get; set; }
        public Warehouse Warehouse { get; set; }

        public List<SO_Product> SO_Products { get; set; }
    }
}
