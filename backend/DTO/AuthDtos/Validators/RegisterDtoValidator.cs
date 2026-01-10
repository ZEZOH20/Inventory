using FluentValidation;
using Inventory.DTO.AuthDtos.Requests;

namespace Inventory.DTO.AuthDtos.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "Owner", "Manager", "Employee" }.Contains(role))
                .WithMessage("Role must be one of: Owner, Manager, Employee");

            RuleFor(x => x.UserKey)
                .NotEmpty().WithMessage("UserKey is required")
                .Must(key => Guid.TryParse(key, out _)).WithMessage("UserKey must be a valid GUID");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP is required")
                .Length(6).WithMessage("OTP must be exactly 6 digits")
                .Matches("^[0-9]{6}$").WithMessage("OTP must contain only digits");
        }
    }
}