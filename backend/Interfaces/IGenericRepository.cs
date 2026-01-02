using System.Linq.Expressions;

namespace Inventory.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQuery();
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<T?> GetByIdAsync(int id);
        T? GetById(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Add(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
    }
}