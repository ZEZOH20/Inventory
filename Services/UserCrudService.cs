using Inventory.Data.DbContexts;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface IUserCrudService
    {
        IEnumerable<User> SelectAll();
    }
    public class UserCrudService : IUserCrudService
    {
        readonly SqlDbContext _conn;
        public UserCrudService(SqlDbContext conn)
        {
           _conn = conn;
        }
        public IEnumerable<User> SelectAll() => _conn.Users.Select(u => u);

    }
}
