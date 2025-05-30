
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models

{
    public class Product
    {
        [Key]
        public int Code { get; set; }
        public required string Name { get; set; }

        [ForeignKey("Unit")]
        public int Unit_id { get; set; }
        public Unit Unit { get; set; }

        public List<Warehouse_Product>? Warehouse_Product { get; set; }
    }
}
