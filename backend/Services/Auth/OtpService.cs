using System.Security.Cryptography;
using System.Text;
using Inventory.DTO.AuthDtos.Responses;
using Microsoft.Extensions.Caching.Distributed;

namespace Inventory.Services.Auth;

public class OtpService : IOtpService
{
    private readonly IDistributedCache _cache;

    public OtpService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<SendVerificationEmailRsDto> GenerateAndStoreOtpAsync(string email, CancellationToken cancellationToken)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        var userKey = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(email)));
        var hashedOtp = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(otp)));
        var expiry = TimeSpan.FromMinutes(5);

        await _cache.SetStringAsync(userKey, hashedOtp, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry
        }, cancellationToken);

        return new SendVerificationEmailRsDto
        {
            Otp = otp,
            UserKey = userKey,
            AvailableUntil = DateTime.UtcNow.Add(expiry)
        };
    }

    public async Task<bool> ValidateOtpAsync(string userKey, string otp, CancellationToken cancellationToken)
    {
        var hashedOtp = await _cache.GetStringAsync(userKey, cancellationToken);
        if (string.IsNullOrEmpty(hashedOtp)) return false;

        var inputHashedOtp = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(otp)));
        return hashedOtp == inputHashedOtp;
    }
}