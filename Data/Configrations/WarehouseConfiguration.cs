using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Inventory.Data.Configrations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            // Each warehouse has only one manager
            builder.HasIndex(w=>w.ManagerId).IsUnique();
        }
    }
}
