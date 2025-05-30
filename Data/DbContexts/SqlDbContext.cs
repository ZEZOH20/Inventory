using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.DbContexts
{
    public class SqlDbContext:DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
            // Your comment here
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Stock_Product

            //modelBuilder.Entity<Warehouse_Product>()
            //    .HasOne(sp => sp.Product)
            //    .WithMany(p => p.Warehouse_Product)
            //    .HasForeignKey(sc => sc.Product_Code);

            //modelBuilder.Entity<Warehouse_Product>()
            //    .HasOne(sp => sp.Warehouse)
            //    .WithMany(w => w.Warehouse_Product)
            //    .HasForeignKey(sp => sp.War_Number);

            // join table name

            //modelBuilder.Entity<Warehouse_Product>().ToTable("Warehouse_Products");

            //Stock_Product
        }
        public DbSet<User> Users { get; set; }
        //public DbSet<Warehouse> Warehouses { get; set; }
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Unit> Units { get; set; }
        //public DbSet<Warehouse_Product> Warehouse_Products { get; set; }
        //public DbSet<SOP>SOPs { get; set; }
        //public DbSet<GI>GIs { get; set; }
        //public DbSet<Transfer> Transfers { get; set; }

    }
}
