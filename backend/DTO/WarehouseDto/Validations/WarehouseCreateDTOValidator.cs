using FluentValidation;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.WarehouseDto.Requests;

namespace Inventory.DTO.WarehouseDto.Validations
{
    public class WarehouseCreateDTOValidator : AbstractValidator<WarehouseCreateDTO>
    {
        public WarehouseCreateDTOValidator()
        {
            RuleFor(x => x.Name)
           .NotEmpty().WithMessage("Name is required")
           .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Region)
                .NotEmpty().WithMessage("Region is required")
                .MaximumLength(50).WithMessage("Region cannot exceed 50 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required")
                .MaximumLength(100).WithMessage("Street cannot exceed 100 characters");

            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Manager ID is required");
        }
    }
}

