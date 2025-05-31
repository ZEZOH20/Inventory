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
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Supplier>Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse_Product> Warehouse_Products { get; set; }
        public DbSet<Supply_Order> Supply_Orders { get; set; }
        public DbSet<Release_Order> Release_Orders { get; set; }
        public DbSet<SO_Product> SO_Products { get; set; }
        public DbSet<RO_Product> RO_Product { get; set; }
        
        //public DbSet<Transfer> Transfers { get; set; }

    }
}
