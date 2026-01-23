using FluentValidation;
using Inventory.DTO.UserDto.Requests;

namespace Inventory.DTO.UserDto.Validations
{
    public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
    {
        public UserUpdateDTOValidator() {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);

            RuleFor(x => x)
                .Must(x => x.HasAtLeastOneValue())
                .WithMessage("At least one field must be provided for update");

            RuleFor(x => x.Name)
            .MaximumLength(100)
            .Unless(x => string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Mail)
                .EmailAddress()
                .Unless(x => string.IsNullOrWhiteSpace(x.Mail))
                .WithMessage("Please enter a valid email address");
        }
    }
}
