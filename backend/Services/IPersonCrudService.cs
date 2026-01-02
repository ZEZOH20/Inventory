using Inventory.Shares;
using Inventory.DTO.UserDto.Requests;
using Inventory.Models;

namespace Inventory.Services
{
    public interface IPersonCrudService<TEntity> where TEntity : Person
    {
        Response<PaginatedResponse<TEntity>> SelectAll(int page = 1, int pageSize = 10);
        Response<bool> UpdateById(UserUpdateDTO dto);
        Response<bool> CheckExistByMail(UserCreateDTO dto);
        Response<bool> Delete(int id);
        Response<TEntity> Create(UserCreateDTO dto);
    }
}