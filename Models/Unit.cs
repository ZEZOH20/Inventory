using System.ComponentModel.DataAnnotations;

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
    public class Unit
    {
        public int Id { get; set; }

        [EnumDataType(typeof(WeightUnit))]
        public required string Name { get; set; }
    }
}
