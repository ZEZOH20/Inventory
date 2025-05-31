using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public enum WUnit
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

        [EnumDataType(typeof(WUnit))]
        public required string Name { get; set; }
    }
}
