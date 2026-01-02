
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models

{
    public enum WeightUnit
    {
        Kilogram,
        Ton,
        Pound,
        Pack,
        Dozen,
        Liter,
        Piece
    }
    public class Product : AuditableEntity
    {
        [Key]
        public int Code { get; set; }
        public required string Name { get; set; }

        [EnumDataType(typeof(WeightUnit), ErrorMessage = "Unit Doesn't Acceptable")]
        public required string Unit { get; set; }

        public List<Warehouse_Product>? Warehouse_Products { get; set; }
        public List<SO_Product> SO_Products { get; set; }
        public List<RO_Product> RO_Products { get; set; }
        public List<TO_Product> TO_Products { get; set; }
    }
}
