
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Inventory.Models
{
    public class Warehouse
    {
        [Key]
        public int Number {  get; set; }

        public string Name { get; set; }
        public string Region {  get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        [ForeignKey("User")]
        public int Manager_ID { get; set; }
        public User Manager { get; set; } //Navigation Property

        public List<Stock_Product>? Stock_Product { get; set; }

    }
}
