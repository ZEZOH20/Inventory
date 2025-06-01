using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface IUserCrudService
    {
        IEnumerable<User> SelectAll();
        bool UpdateById(UserUpdateDTO dto);
        bool CheckExistByMail(UserCreateDTO dto);
        bool  Delete(int id);    

        void Create(UserCreateDTO dto);
    }
    public class UserCrudService : IUserCrudService
    {
        readonly SqlDbContext _conn;
        public UserCrudService(SqlDbContext conn)
        {
           _conn = conn;
        }
        public IEnumerable<User> SelectAll() => _conn.Users.Select(u => u);

        public bool CheckExistByMail(UserCreateDTO dto) => _conn.Users.Any(u => u.Mail == dto.Mail);

        public void Create(UserCreateDTO dto)
        {
            _conn.Users.Add(new User
            {
                Name = dto.Name,
                Domain = dto.Domain,
                Phone = int.Parse(dto.Phone),
                Fax = dto.Fax,
                Mail = dto.Mail
            });
            _conn.SaveChanges();
        }
        public bool UpdateById(UserUpdateDTO dto)
        {
            var user = _conn.Users.FirstOrDefault(u => u.Id == dto.Id);

            if (user == null)
                return false;

            // Update only provided values
            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                user.Phone = int.Parse(dto.Phone);

            if (!string.IsNullOrEmpty(dto.Fax)) 
                user.Fax = dto.Fax;

            if (!string.IsNullOrEmpty(dto.Mail))
                user.Mail = dto.Mail;

            if (!string.IsNullOrEmpty(dto.Domain)) 
                user.Domain = dto.Domain;

            _conn.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var user = _conn.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _conn.Users.Remove(user);
                _conn.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
