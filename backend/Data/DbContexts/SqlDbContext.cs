
//using backend.Data.Configrations;
using backend.Data.Configrations;
using backend.Models;
using Inventory.Data.Configrations;
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
            modelBuilder.ApplyConfiguration(new WarehouseConfig());
            modelBuilder.ApplyConfiguration(new Warehouse_ProductConfig(modelBuilder));
            modelBuilder.ApplyConfiguration(new SO_ProductConfig(modelBuilder));
            modelBuilder.ApplyConfiguration(new RO_ProductConfig(modelBuilder));
            modelBuilder.ApplyConfiguration(new TO_ProductConfig(modelBuilder));

            modelBuilder.Entity<Transfer_Order>()
             .HasOne(t => t.FromWarehouse)
             .WithMany()
             .HasForeignKey(t => t.From)
             .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Supplier>Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse_Product> Warehouse_Products { get; set; }
        public DbSet<Supply_Order> Supply_Orders { get; set; }
        public DbSet<Release_Order> Release_Orders { get; set; }
        public DbSet<Transfer_Order> Transfer_Orders { get; set; }
        public DbSet<SO_Product> SO_Products { get; set; }
        public DbSet<RO_Product> RO_Product { get; set; }
        public DbSet<TO_Product> TO_Products { get; set; }
    

    }
}
