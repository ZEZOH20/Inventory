using Inventory.Data.DbContexts;
using Inventory.Interfaces;
using Inventory.Models;

namespace Inventory.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlDbContext _context;

        public UnitOfWork(SqlDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Products = new GenericRepository<Product>(_context);
            WarehouseProducts = new GenericRepository<Warehouse_Product>(_context);
            SupplyOrders = new GenericRepository<Supply_Order>(_context);
            ReleaseOrders = new GenericRepository<Release_Order>(_context);
            TransferOrders = new GenericRepository<Transfer_Order>(_context);
            SOProducts = new GenericRepository<SO_Product>(_context);
            ROProducts = new GenericRepository<RO_Product>(_context);
            TOProducts = new GenericRepository<TO_Product>(_context);
        }

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Supplier> Suppliers { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Warehouse> Warehouses { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Warehouse_Product> WarehouseProducts { get; }
        public IGenericRepository<Supply_Order> SupplyOrders { get; }
        public IGenericRepository<Release_Order> ReleaseOrders { get; }
        public IGenericRepository<Transfer_Order> TransferOrders { get; }
        public IGenericRepository<SO_Product> SOProducts { get; }
        public IGenericRepository<RO_Product> ROProducts { get; }
        public IGenericRepository<TO_Product> TOProducts { get; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public int Complete() => _context.SaveChanges();
    }
}