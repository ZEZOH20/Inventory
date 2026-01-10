using Inventory.DTO.AuthDtos.Responses;

namespace Inventory.Services.Auth;

public interface IOtpService
{
    Task<SendVerificationEmailRsDto> GenerateAndStoreOtpAsync(string email, CancellationToken cancellationToken);
    Task<bool> ValidateOtpAsync(string userKey, string otp, CancellationToken cancellationToken);
}