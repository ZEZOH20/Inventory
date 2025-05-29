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

            modelBuilder.Entity<Stock_Product>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.Stock_Product)
                .HasForeignKey(sc => sc.Product_Code);

            modelBuilder.Entity<Stock_Product>()
                .HasOne(sp => sp.Warehouse)
                .WithMany(w => w.Stock_Product)
                .HasForeignKey(sp => sp.War_Number);

            // join table name
            modelBuilder.Entity<Stock_Product>().ToTable("Stock_Products");

            //Stock_Product
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Stock_Product> Stock_Products { get; set; }
        public DbSet<SOP>SOPs { get; set; }
        public DbSet<GI>GIs { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
      
    }
}
