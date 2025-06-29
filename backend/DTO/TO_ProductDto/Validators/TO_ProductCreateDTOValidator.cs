using backend.DTO.TO_ProductDto.Requests;
using FluentValidation;
using Inventory.Data.DbContexts;

namespace backend.DTO.TO_ProductDto.Validators
{
    public class TO_ProductCreateDTOValidator: AbstractValidator<TO_ProductCreateDTO>
    {
        SqlDbContext _conn;
        public TO_ProductCreateDTOValidator(SqlDbContext conn) {

            _conn = conn;
            RuleFor(x => x.TO_Amount)
            .NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.TO_MFD)
                .NotEmpty().WithMessage("Manufacturing date is required");

            RuleFor(x => x.TO_EXP)
                .NotEmpty().WithMessage("Expiry date is required");

            RuleFor(x => x.TO_Number)
                .NotEmpty().WithMessage("Transfer Order Number is required")
                .Must((dto, toNumber) => _conn.Transfer_Orders.Any(to => to.Number == toNumber))
                .WithMessage("Transfer Order doesn't exist to insert products to it");

            RuleFor(x => x.Product_Code)
                .NotEmpty().WithMessage("Product Code is required")
                .GreaterThan(0);
        }
    }
}
