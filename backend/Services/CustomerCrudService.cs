using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface ICustomerCrudService : IPersonCrudService<Customer>
    {
        Response<PaginatedResponse<Customer>> SelectAll(int page = 1, int pageSize = 10);
        Response<bool> UpdateById(UserUpdateDTO dto);
        Response<bool> CheckExistByMail(UserCreateDTO dto);
        Response<bool> Delete(int id);
        Response<Customer> Create(UserCreateDTO dto);
    }
    public class CustomerCrudService : ICustomerCrudService
    {
        readonly IUnitOfWork _unitOfWork;
        public CustomerCrudService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Response<PaginatedResponse<Customer>> SelectAll(int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.Customers.GetQuery();
            var totalCount = query.Count();
            var customers = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginated = new PaginatedResponse<Customer>(customers, page, pageSize, totalCount);
            return Response<PaginatedResponse<Customer>>.Success(paginated);
        }

        public Response<bool> CheckExistByMail(UserCreateDTO dto)
        {
            var exists = _unitOfWork.Customers.Find(c => c.Mail == dto.Mail).Any();
            return Response<bool>.Success(exists);
        }

        public Response<Customer> Create(UserCreateDTO dto)
        {
            try
            {
                var customer = new Customer
                {
                    Name = dto.Name,
                    Domain = dto.Domain,
                    Phone = int.Parse(dto.Phone),
                    Fax = dto.Fax,
                    Mail = dto.Mail
                };
                _unitOfWork.Customers.Add(customer);
                _unitOfWork.Complete();
                return Response<Customer>.Success(customer);
            }
            catch (Exception ex)
            {
                return Response<Customer>.Failure("Can't create customer: " + ex.Message);
            }
        }
        public Response<bool> UpdateById(UserUpdateDTO dto)
        {
            try
            {
                var customer = _unitOfWork.Customers.GetById(dto.Id);

                if (customer == null)
                    return Response<bool>.Failure("Customer not found");

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

                _unitOfWork.Complete();

                return Response<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure("Can't update customer: " + ex.Message);
            }
        }

        public Response<bool> Delete(int id)
        {
            try
            {
                var customer = _unitOfWork.Customers.GetById(id);
                if (customer != null)
                {
                    _unitOfWork.Customers.Remove(customer);
                    _unitOfWork.Complete();
                    return Response<bool>.Success(true);
                }
                return Response<bool>.Failure("Customer not found");
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure("Can't delete customer: " + ex.Message);
            }
        }
    }
}
