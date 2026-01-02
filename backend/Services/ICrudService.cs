using Inventory.Shares;

namespace Inventory.Services
{
    public interface ICrudService<TEntity, TCreateDto, TUpdateDto, TResponseDto>
        where TEntity : class
    {
        Response<PaginatedResponse<TEntity>> SelectAll(int page = 1, int pageSize = 10);
        Response<TEntity> SelectById(int id);
        Response<TEntity> Insert(TCreateDto dto);
        Response<bool> Update(int id, TUpdateDto dto);
        Response<bool> Delete(int id);
    }
}