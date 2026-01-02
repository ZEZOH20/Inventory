using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Requests;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventory.Services
{
    public interface IUserCrudService : IPersonCrudService<User>
    {
        Response<PaginatedResponse<User>> SelectAll(int page = 1, int pageSize = 10);
        Response<bool> UpdateById(UserUpdateDTO dto);
        Response<bool> CheckExistByMail(UserCreateDTO dto);
        Response<bool> Delete(int id);

        Response<User> Create(UserCreateDTO dto);
    }
    public class UserCrudService : IUserCrudService
    {
        readonly IUnitOfWork _unitOfWork;
        public UserCrudService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Response<PaginatedResponse<User>> SelectAll(int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.Users.GetQuery();
            var totalCount = query.Count();
            var users = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginated = new PaginatedResponse<User>(users, page, pageSize, totalCount);
            return Response<PaginatedResponse<User>>.Success(paginated);
        }

        public Response<bool> CheckExistByMail(UserCreateDTO dto)
        {
            var exists = _unitOfWork.Users.Find(u => u.Mail == dto.Mail).Any();
            return Response<bool>.Success(exists);
        }

        public Response<User> Create(UserCreateDTO dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Domain = dto.Domain,
                Phone = int.Parse(dto.Phone),
                Fax = dto.Fax,
                Mail = dto.Mail
            };
            _unitOfWork.Users.Add(user);
            _unitOfWork.Complete();
            return Response<User>.Success(user);
        }
        public Response<bool> UpdateById(UserUpdateDTO dto)
        {
            var user = _unitOfWork.Users.GetById(dto.Id);

            if (user == null)
                return Response<bool>.Failure("User not found");

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

            _unitOfWork.Complete();

            return Response<bool>.Success(true);
        }

        public Response<bool> Delete(int id)
        {
            var user = _unitOfWork.Users.GetById(id);
            if (user != null)
            {
                _unitOfWork.Users.Remove(user);
                _unitOfWork.Complete();
                return Response<bool>.Success(true);
            }
            return Response<bool>.Failure("User not found");
        }
    }
}
