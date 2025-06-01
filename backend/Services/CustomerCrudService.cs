using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface ICustomerCrudService
    {
        IEnumerable<Customer> SelectAll();
        bool UpdateById(UserUpdateDTO dto);
        bool CheckExistByMail(UserCreateDTO dto);
        bool Delete(int id);

        void Create(UserCreateDTO dto);
    }
    public class CustomerCrudService : ICustomerCrudService
    {
        readonly SqlDbContext _conn;
        public CustomerCrudService(SqlDbContext conn)
        {
            _conn = conn;
        }
        public IEnumerable<Customer> SelectAll() => _conn.Customers.Select(u => u);

        public bool CheckExistByMail(UserCreateDTO dto) => _conn.Customers.Any(u => u.Mail == dto.Mail);

        public void Create(UserCreateDTO dto)
        {
            _conn.Customers.Add(new Customer
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
            var customer = _conn.Customers.FirstOrDefault(u => u.Id == dto.Id);

            if (customer == null)
                return false;

            // Update only provided values
            if (!string.IsNullOrEmpty(dto.Name))
                customer.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                customer.Phone = int.Parse(dto.Phone);

            if (!string.IsNullOrEmpty(dto.Fax))
                customer.Fax = dto.Fax;

            if (!string.IsNullOrEmpty(dto.Mail))
                customer.Mail = dto.Mail;

            if (!string.IsNullOrEmpty(dto.Domain))
                customer.Domain = dto.Domain;

            _conn.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var customer = _conn.Customers.FirstOrDefault(u => u.Id == id);
            if (customer != null)
            {
                _conn.Customers.Remove(customer);
                _conn.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
