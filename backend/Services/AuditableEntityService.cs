using Inventory.Data.DbContexts;
using Inventory.Services.CurrentUser;

namespace Inventory.Services;

public class AuditableEntityService : IAuditableEntityService
{
    private readonly SqlDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AuditableEntityService(SqlDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task SoftDeleteAsync<T>(T entity) where T : Models.AuditableEntity
    {
        entity.SoftDelete(_currentUser.UserId, _currentUser.GetUserIp());
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync<T>(T entity) where T : Models.AuditableEntity
    {
        entity.Restore();
        await _context.SaveChangesAsync();
    }
}