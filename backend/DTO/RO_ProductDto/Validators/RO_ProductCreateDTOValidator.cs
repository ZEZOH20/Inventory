using FluentValidation;
using Inventory.Data.DbContexts;
using Inventory.DTO.RO_ProductDto.Requests;

namespace Inventory.DTO.RO_ProductDto.Validators
{
    public class RO_ProductCreateDTOValidator:AbstractValidator<RO_ProductCreateDTO>
    {
        SqlDbContext _conn;
        public RO_ProductCreateDTOValidator(SqlDbContext conn)
        {
            _conn = conn;

            RuleFor(x => x.RO_Number)
                .NotEmpty().WithMessage("Release Order Number is required")
                .Must((dto, roNumber) => _conn.Release_Orders.Any(ro => ro.Number == roNumber))
                .WithMessage("Release Order doesn't exist to Release products");

            RuleFor(x => x.WarehouseProduct_Id)
             .NotEmpty().WithMessage("Warehouse Product Id is required")
             .Must((dto, WarehouseProduct_Id) => _conn.Warehouse_Products.Any(wp => wp.Id == WarehouseProduct_Id))
             .WithMessage("Warehouse Product doesn't exist to release");

        }
    }
}
