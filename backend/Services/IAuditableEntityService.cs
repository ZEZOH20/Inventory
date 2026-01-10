namespace Inventory.Services;

public interface IAuditableEntityService
{
    Task SoftDeleteAsync<T>(T entity) where T : Models.AuditableEntity;
    Task RestoreAsync<T>(T entity) where T : Models.AuditableEntity;
}