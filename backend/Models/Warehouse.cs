
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Inventory.Models
{
    public class Warehouse : AuditableEntity
    {
        [Key]
        public int Number { get; set; }
        public required string Name { get; set; }
        public required string Region { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }

        [ForeignKey("Manager")]
        public required string ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; } //Navigation Property

        public List<Warehouse_Product>? Warehouse_Products { get; set; }

    }
}
