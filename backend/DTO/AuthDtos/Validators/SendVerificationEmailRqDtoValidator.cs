using FluentValidation;
using Inventory.DTO.AuthDtos.Requests;

namespace Inventory.DTO.AuthDtos.Validators
{
    public class SendVerificationEmailRqDtoValidator : AbstractValidator<SendVerificationEmailRqDto>
    {
        public SendVerificationEmailRqDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}