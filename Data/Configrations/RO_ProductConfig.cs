using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Data.Configrations
{
    public class RO_ProductConfig : IEntityTypeConfiguration<RO_Product>
    {
        ModelBuilder _ModelBuilder;
        public RO_ProductConfig(ModelBuilder ModelBuilder)
        {
            _ModelBuilder = ModelBuilder;
            CreateTable();
        }

        private void CreateTable()
        {
            //Relation between RO_Product and Release_Order
            _ModelBuilder.Entity<RO_Product>()
                .HasOne(ro => ro.Release_Order)
                .WithMany(s => s.RO_Products)
                .HasForeignKey(so => so.RO_Number);

            //Relation between RO_Product and Product
            _ModelBuilder.Entity<RO_Product>()
               .HasOne(ro => ro.Product)
               .WithMany(p => p.RO_Products)
               .HasForeignKey(ro => ro.Product_Code);
        }

        public void Configure(EntityTypeBuilder<RO_Product> builder)
        {
            //
        }
    }
}
