namespace Inventory.Services.CurrentUser;

public interface ICurrentUser
{
    string? UserId { get; }
    string? GetUserIp();
}