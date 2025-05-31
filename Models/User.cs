using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class User : Person
    {
       public Warehouse Warehouse { get; set; } //Navigation
    }
}
