using System.ComponentModel.DataAnnotations;
using Inventory.Shares;

namespace Inventory.Models;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public string? CreatedIP { get; private set; }
    public string? UpdatedIP { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    [Timestamp]
    public byte[] RowVersion { get; private set; } = null!;

    public void SetCreated(string userId, string? ip = null)
    {
        CreatedBy = userId;
        CreatedIP = ip?.Truncate(45);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdated(string userId, string? ip = null)
    {
        UpdatedBy = userId;
        UpdatedIP = ip?.Truncate(45);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete(string userId, string? ip = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
        SetUpdated(userId, ip);
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}