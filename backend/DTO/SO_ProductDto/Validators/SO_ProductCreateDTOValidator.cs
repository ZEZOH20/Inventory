using FluentValidation;
using Inventory.Data.DbContexts;
using Inventory.DTO.SO_ProductDto.Requests;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Inventory.DTO.SO_ProductDto.Validators
{
    public class SO_ProductCreateDTOValidator:AbstractValidator<SO_ProductCreateDTO>
    {
        readonly SqlDbContext _conn;
        public SO_ProductCreateDTOValidator(SqlDbContext conn)
        {
            _conn = conn;
            RuleFor(x => x.SO_Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.SO_Unit)
                .NotEmpty().WithMessage("Unit is required");

            RuleFor(x => x.SO_Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.SO_MFD)
                .NotEmpty().WithMessage("Manufacturing date is required");

            RuleFor(x => x.SO_EXP)
                .NotEmpty().WithMessage("Expiry date is required");

            RuleFor(x => x.SO_Number)
                .NotEmpty().WithMessage("Supply Order Number is required")
                .Must((dto, soNumber) => _conn.Supply_Orders.Any(so => so.Number == soNumber))
                .WithMessage("Supply Order doesn't exist to insert products to it");

            RuleFor(x => x.Product_Code)
                .NotEmpty().WithMessage("Product Code is required")
                .GreaterThan(0);
        }
    }
}
