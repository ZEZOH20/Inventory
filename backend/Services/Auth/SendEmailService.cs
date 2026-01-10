using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Inventory.Services.Auth;

public class SendEmailService : ISendEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public SendEmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task<bool> SendVerificationEmail(string email, string otp)
    {
        var emailMessage = await _fluentEmail
            .To(email)
            .Subject("Email Verification")
            .Body($"Your OTP for email verification is: {otp}. It expires in 5 minutes.")
            .SendAsync();

        return emailMessage.Successful;
    }

    public async Task<bool> SendEmailAsync(string email, string subject, string body)
    {
        var emailMessage = await _fluentEmail
            .To(email)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync();

        return emailMessage.Successful;
    }
}