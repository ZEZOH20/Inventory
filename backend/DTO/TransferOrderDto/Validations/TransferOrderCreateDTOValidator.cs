using backend.DTO.TransferOrderDto.Requests;
using FluentValidation;
using Inventory.Data.DbContexts;

namespace backend.DTO.TransferOrderDto.Validations
{
    public class TransferOrderCreateDTOValidator:AbstractValidator<TransferOrderCreateDto>
    {
        readonly SqlDbContext _conn;
        public TransferOrderCreateDTOValidator(SqlDbContext conn)
        {
            _conn = conn;
            RuleFor(x => x.Supplier_ID)
              .NotEmpty().WithMessage("Supplier Id is required")
              .Must((dto, SupplierID) => _conn.Suppliers.Any(s => s.Id == SupplierID))
               .WithMessage("Supplier doesn't exists");

            RuleFor(x=>x.From)
                .NotEmpty().WithMessage("Warehouse Number is required")
                .Must((dto, warNumber) => _conn.Warehouses.Any(to => to.Number == warNumber))
                 .WithMessage("Warehouse doesn't exist to Transfer form it");

            RuleFor(x => x.To)
             .NotEmpty().WithMessage("Warehouse Number is required")
             .Must((dto, warNumber) => _conn.Warehouses.Any(to => to.Number == warNumber))
             .WithMessage("Warehouse doesn't exist to Transfer form it");


            RuleFor(x => x)
                .Must(dto => dto.From != dto.To)
                .WithMessage("'From' and 'To' warehouse numbers cannot be the same.");
        }
    }
}
