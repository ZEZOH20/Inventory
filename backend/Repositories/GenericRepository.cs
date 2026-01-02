using System.Linq.Expressions;
using Inventory.Data.DbContexts;
using Inventory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SqlDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SqlDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQuery() => _dbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public T? GetById(int id) => _dbSet.Find(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public IEnumerable<T> GetAll() => _dbSet.ToList();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public T? FirstOrDefault(Expression<Func<T, bool>> predicate)
            => _dbSet.FirstOrDefault(predicate);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
            => _dbSet.Where(predicate);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Add(T entity) => _dbSet.Add(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbSet.AddRangeAsync(entities);

        public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

        public void Update(T entity) => _context.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    }
}