namespace Inventory.Models
{
    public class Supplier: Person
    {
        public List<Warehouse_Product>? Warehouse_Products { get; set; }

        public Supply_Order Supply_Order { get; set; }
    }
}
