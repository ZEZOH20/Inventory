using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface ISupplierCrudService
    {
        IEnumerable<Supplier> SelectAll();
        bool UpdateById(UserUpdateDTO dto);
        bool CheckExistByMail(UserCreateDTO dto);
        bool Delete(int id);

        void Create(UserCreateDTO dto);
    }
    public class SupplierCrudService : ISupplierCrudService
    {
        readonly SqlDbContext _conn;
        public SupplierCrudService(SqlDbContext conn)
        {
            _conn = conn;
        }
        public IEnumerable<Supplier> SelectAll() => _conn.Suppliers.Select(u => u);

        public bool CheckExistByMail(UserCreateDTO dto) => _conn.Suppliers.Any(u => u.Mail == dto.Mail);

        public void Create(UserCreateDTO dto)
        {
            _conn.Suppliers.Add(new Supplier
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
            var supplier = _conn.Suppliers.FirstOrDefault(u => u.Id == dto.Id);

            if (supplier == null)
                return false;

            // Update only provided values
            if (!string.IsNullOrEmpty(dto.Name))
                supplier.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                supplier.Phone = int.Parse(dto.Phone);

            if (!string.IsNullOrEmpty(dto.Fax))
                supplier.Fax = dto.Fax;

            if (!string.IsNullOrEmpty(dto.Mail))
                supplier.Mail = dto.Mail;

            if (!string.IsNullOrEmpty(dto.Domain))
                supplier.Domain = dto.Domain;

            _conn.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var supplier = _conn.Suppliers.FirstOrDefault(u => u.Id == id);
            if (supplier != null)
            {
                _conn.Suppliers.Remove(supplier);
                _conn.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
