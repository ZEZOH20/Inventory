using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inventory.Services.CurrentUser;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetUserIp()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        // Try to get IP from X-Forwarded-For header (for proxies)
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
        {
            // Take the first IP if multiple
            ip = ip.Split(',')[0].Trim();
        }
        else
        {
            // Fallback to remote IP
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        return ip;
    }
}