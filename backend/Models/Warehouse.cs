
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Inventory.Models
{
    public class Warehouse
    {
        [Key]
        public int Number { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        [ForeignKey("Manager")]
        public string ManagerId { get; set; }
        public ApplicationUser Manager { get; set; } //Navigation Property

        public List<Warehouse_Product>? Warehouse_Products { get; set; }

    }
}
