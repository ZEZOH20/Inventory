using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Inventory.Data.Configrations
{
    public class Warehouse_ProductConfig : IEntityTypeConfiguration<Warehouse_Product>
    {
        ModelBuilder _ModelBuilder;
        public Warehouse_ProductConfig(ModelBuilder ModelBuilder)
        {
            _ModelBuilder = ModelBuilder;
            CreateTable();
        }

        public void CreateTable()
        {
            //Warehouse_Product

            _ModelBuilder.Entity<Warehouse_Product>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.Warehouse_Products)
                .HasForeignKey(sc => sc.Product_Code);

            _ModelBuilder.Entity<Warehouse_Product>()
                .HasOne(sp => sp.Warehouse)
                .WithMany(w => w.Warehouse_Products)
                .HasForeignKey(sp => sp.War_Number);

            _ModelBuilder.Entity<Warehouse_Product>()
                .HasOne(sp => sp.Supplier)
                .WithMany(s => s.Warehouse_Products)
                .HasForeignKey(sp => sp.Supplier_ID);

            // join table name
            _ModelBuilder.Entity<Warehouse_Product>().ToTable("Warehouse_Products");
        }
        public void Configure(EntityTypeBuilder<Warehouse_Product> builder)
        {
            builder.Property(sp => sp.Product_Code)
                   .IsRequired();
                   
        }
    }
}
