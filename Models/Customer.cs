namespace Inventory.Models
{
    public class Customer: Person
    {
        public Release_Order Release_Order { get; set; }
    }
}
