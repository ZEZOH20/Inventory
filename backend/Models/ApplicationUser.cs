using Microsoft.AspNetCore.Identity;

namespace Inventory.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? WarehouseId { get; set; } // For Managers, nullable for others

        // Navigation property
        public Warehouse? Warehouse { get; set; }
    }
}