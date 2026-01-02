using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface ISupplierCrudService : IPersonCrudService<Supplier>
    {
        Response<PaginatedResponse<Supplier>> SelectAll(int page = 1, int pageSize = 10);
        Response<bool> UpdateById(UserUpdateDTO dto);
        Response<bool> CheckExistByMail(UserCreateDTO dto);
        Response<bool> Delete(int id);

        Response<Supplier> Create(UserCreateDTO dto);
    }
    public class SupplierCrudService : ISupplierCrudService
    {
        readonly IUnitOfWork _unitOfWork;
        public SupplierCrudService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Response<PaginatedResponse<Supplier>> SelectAll(int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.Suppliers.GetQuery();
            var totalCount = query.Count();
            var suppliers = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginated = new PaginatedResponse<Supplier>(suppliers, page, pageSize, totalCount);
            return Response<PaginatedResponse<Supplier>>.Success(paginated);
        }

        public Response<bool> CheckExistByMail(UserCreateDTO dto)
        {
            var exists = _unitOfWork.Suppliers.Find(u => u.Mail == dto.Mail).Any();
            return Response<bool>.Success(exists);
        }

        public Response<Supplier> Create(UserCreateDTO dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Domain = dto.Domain,
                Phone = int.Parse(dto.Phone),
                Fax = dto.Fax,
                Mail = dto.Mail
            };
            _unitOfWork.Suppliers.Add(supplier);
            _unitOfWork.Complete();
            return Response<Supplier>.Success(supplier);
        }
        public Response<bool> UpdateById(UserUpdateDTO dto)
        {
            var supplier = _unitOfWork.Suppliers.GetById(dto.Id);

            if (supplier == null)
                return Response<bool>.Failure("Supplier not found");

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

            _unitOfWork.Complete();

            return Response<bool>.Success(true);
        }

        public Response<bool> Delete(int id)
        {
            var supplier = _unitOfWork.Suppliers.GetById(id);
            if (supplier != null)
            {
                _unitOfWork.Suppliers.Remove(supplier);
                _unitOfWork.Complete();
                return Response<bool>.Success(true);
            }
            return Response<bool>.Failure("Supplier not found");
        }
    }
}
