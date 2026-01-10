using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Inventory.Shares;

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

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ReviewNotes { get; set; }
        public string? CancellationReason { get; set; }

        //Navigation
        public Supplier Supplier { get; set; }
        public Warehouse Warehouse { get; set; }

        public List<SO_Product> SO_Products { get; set; }
    }
}
