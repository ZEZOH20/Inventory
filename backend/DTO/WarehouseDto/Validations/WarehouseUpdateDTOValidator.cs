using FluentValidation;
using Inventory.DTO.WarehouseDto.Requests;

namespace Inventory.DTO.WarehouseDto.Validations
{
    public class WarehouseUpdateDTOValidator: AbstractValidator<WarehouseUpdateDTO>
    {
        public WarehouseUpdateDTOValidator() {
            RuleFor(x => x.Number).NotEmpty().GreaterThan(0);

            RuleFor(x => x)
                .Must(x => x.HasAtLeastOneValue())
                .WithMessage("At least one field must be provided for update");

            RuleFor(x => x.Name)
            .MaximumLength(100)
            .Unless(x => string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Name is to mach ");

            RuleFor(x => x.City)
                .MaximumLength(50)
                .Unless(x => string.IsNullOrWhiteSpace(x.City))
                .WithMessage("City cannot exceed 50 characters");

            RuleFor(x => x.Region)
                .MaximumLength(50)
                .Unless(x => string.IsNullOrWhiteSpace(x.City))
                .WithMessage("City cannot exceed 50 characters");

            RuleFor(x => x.Street)
             .MaximumLength(100)
             .Unless(x => string.IsNullOrWhiteSpace(x.City))
             .WithMessage("City cannot exceed 50 characters");


        }
    }
}
