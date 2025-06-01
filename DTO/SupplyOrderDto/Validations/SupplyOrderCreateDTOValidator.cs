using FluentValidation;
using Inventory.DTO.SupplyOrderDto.Requests;

namespace Inventory.DTO.SupplyOrderDto.Validations
{
    public class SupplyOrderCreateDTOValidator: AbstractValidator<SupplyOrderCreateDTO>
    {
        public SupplyOrderCreateDTOValidator()
        {
            RuleFor(x => x.Supplier_ID)
              .NotEmpty().WithMessage("Supplier ID is required")
              .GreaterThan(0).WithMessage("Supplier ID must be greater than 0");

            RuleFor(x => x.War_Number)
           .NotEmpty().WithMessage("Warehouse Number is required")
           .GreaterThan(0).WithMessage("Warehouse Number must be greater than 0");
        }
    }
}
