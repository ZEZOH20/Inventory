using FluentValidation;
using Inventory.DTO.UserDto.Requests;
using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.UserDto.Validations
{
    public class UserCreateDTOValidator : AbstractValidator<UserCreateDTO>
    {
        public UserCreateDTOValidator()
        {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Name is required")
             .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\d+$").WithMessage("Phone must contain only digits");

            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address");

            // Optional fields
            When(x => !string.IsNullOrWhiteSpace(x.Fax), () =>
            {
                RuleFor(x => x.Fax)
                    .MaximumLength(50).WithMessage("Fax cannot exceed 50 characters");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Domain), () =>
            {
                RuleFor(x => x.Domain)
                    .MaximumLength(100).WithMessage("Domain cannot exceed 100 characters");
            });

        }
    }
}
