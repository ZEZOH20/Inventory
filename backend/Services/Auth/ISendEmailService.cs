namespace Inventory.Services.Auth;

public interface ISendEmailService
{
    Task<bool> SendVerificationEmail(string email, string otp);
    Task<bool> SendEmailAsync(string email, string subject, string body);
}