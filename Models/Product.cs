
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
    public class Product
    {
        [Key]
        public int Code { get; set; }
        public required string Name { get; set; }

        [EnumDataType(typeof(WeightUnit),ErrorMessage = "Unit Doesn't Acceptable")]
        public required string Unit { get; set; }

        public List<Warehouse_Product>? Warehouse_Product { get; set; }
    }
}
