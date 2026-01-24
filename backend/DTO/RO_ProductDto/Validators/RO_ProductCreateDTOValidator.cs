using FluentValidation;
using Inventory.Data.DbContexts;
using Inventory.DTO.RO_ProductDto.Requests;

namespace Inventory.DTO.RO_ProductDto.Validators
{
    public class RO_ProductCreateDTOValidator : AbstractValidator<RO_ProductCreateDTO>
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

            RuleFor(x => x.RO_Amount)
                .GreaterThan(0).WithMessage("Release amount must be greater than 0")
                .Must((dto, roAmount) =>
                {
                    var wp = _conn.Warehouse_Products.FirstOrDefault(w => w.Id == dto.WarehouseProduct_Id);
                    return wp != null && roAmount <= wp.Total_Amount - wp.ReservedQuantity;
                })
                .WithMessage("Release amount exceeds available quantity in warehouse product");

        }
    }
}
