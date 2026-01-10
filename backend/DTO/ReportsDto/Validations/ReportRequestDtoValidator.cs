using FluentValidation;
using Inventory.DTO.ReportsDto.Requests;

namespace Inventory.DTO.ReportsDto.Validations
{
    public class ReportRequestDtoValidator : AbstractValidator<ReportRequestDto>
    {
        public ReportRequestDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start date cannot be in the future");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after or equal to start date")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("End date cannot be in the future");

            RuleFor(x => x.WarehouseId)
                .GreaterThan(0).When(x => x.WarehouseId.HasValue).WithMessage("Warehouse ID must be greater than 0");
        }
    }
}