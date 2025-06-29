using backend.Models;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Data.Configrations
{
    public class TO_ProductConfig : IEntityTypeConfiguration<TO_Product>
    {
        ModelBuilder _ModelBuilder;
        public TO_ProductConfig(ModelBuilder ModelBuilder)
        {
            _ModelBuilder = ModelBuilder;
            CreateTable();
        }

        void CreateTable()
        {
            //Relation between TO_Product and Transfer_Order
            _ModelBuilder.Entity<TO_Product>()
                .HasOne(to => to.Transfer_Order)
                .WithMany(t => t.TO_Products)
                .HasForeignKey(to => to.TO_Number);

            //Relation between TO_Product and Product
            _ModelBuilder.Entity<TO_Product>()
               .HasOne(to => to.Product)
               .WithMany(p => p.TO_Products)
               .HasForeignKey(to => to.Product_Code);
        }
        public void Configure(EntityTypeBuilder<TO_Product> builder)
        {
            //
        }
    }
}
