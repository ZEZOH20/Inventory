using FluentValidation;
using Inventory.DTO.ReleaseOrderDto.Requests;

namespace Inventory.DTO.ReleaseOrderDto.Validators
{
    public class ReleaseOrderCreateDTOValidator:AbstractValidator<ReleaseOrderCreateDTO>
    {
        public ReleaseOrderCreateDTOValidator()
        {
            RuleFor(x => x.Customer_ID)
           .NotEmpty().WithMessage("Customer ID is required")
           .GreaterThan(0).WithMessage("Customer ID must be greater than 0");

            RuleFor(x => x.War_Number)
           .NotEmpty().WithMessage("Warehouse Number is required")
           .GreaterThan(0).WithMessage("Warehouse Number must be greater than 0");
        }
    }
}
