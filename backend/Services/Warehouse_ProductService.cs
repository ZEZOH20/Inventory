using Inventory.Data.DbContexts;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Interfaces;
using Inventory.Models;
using Inventory.Shares;
using Inventory.Services.CurrentUser;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public interface IWarehouse_ProductService
    {
        Response CreateWarehouse_Product(Warehouse_ProductCreateDTO dto);
        bool CreationIsValid(Warehouse_ProductCreateDTO dto);
        Warehouse_Product? ProductExistInWarehouse(Warehouse_ProductCreateDTO dto, DateTime mfdDate, DateTime expDate);
        Warehouse_Product? ProductExistInWarehouse(int Supplier_ID, int Product_Code, int War_Number, DateTime mfdDate, DateTime expDate);
        Response<Warehouse_Product> Delete(int Id);
    }
    public class Warehouse_ProductService : IWarehouse_ProductService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ICurrentUser _currentUser;

        public Warehouse_ProductService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }


        public Response CreateWarehouse_Product(Warehouse_ProductCreateDTO dto)
        {

            //validation
            if (!dto.Valid())
                return Response.Failure($"check: \n" +
                    $"Product_Code,\n" +
                    $"War_Number,\n" +
                    $"Amount,\n" +
                    $"Price,\n" +
                    $"greater than > 0 \n\n" +
                    $"MFD , EXP , EXP are Date Type");

            if (!CreationIsValid(dto))
                return Response.Failure($"Supplier_ID or \n " +
                    $"Product_Code or \n " +
                    $"Warehouse_Number \n  " +
                    "can't found check them and try later");

            // Parse and validate dates
            DateTime mfdDate = DateTime.Parse(dto.MFD);
            DateTime expDate = DateTime.Parse(dto.EXP);
            if (expDate <= mfdDate)
                return Response.Failure($"EXP Date : {expDate} \n " +
                    $"can't be less than or equal\n" +
                    $"MFD Date : {mfdDate}");
            //validation

            try
            {
                var existingProduct = ProductExistInWarehouse(dto, mfdDate, expDate);
                //var existingProduct = ProductExistInWarehouse(dto.Supplier_ID, dto.Product_Code, dto.War_Number, mfdDate , expDate);

                if (existingProduct is not null)
                {
                    existingProduct.Total_Amount += dto.Amount;
                    existingProduct.Total_Price += dto.Amount * dto.Price;
                    existingProduct.SetUpdated(_currentUser.UserId);
                    _unitOfWork.WarehouseProducts.Update(existingProduct);
                }
                else
                {
                    var newProduct = new Warehouse_Product
                    {
                        War_Number = dto.War_Number,
                        Product_Code = dto.Product_Code,
                        Supplier_ID = dto.Supplier_ID,
                        MFD = mfdDate,
                        EXP = expDate,
                        Store_Date = DateTime.UtcNow,
                        Total_Amount = dto.Amount,
                        Total_Price = dto.Amount * dto.Price,
                    };
                    newProduct.SetCreated(_currentUser.UserId);
                    _unitOfWork.WarehouseProducts.AddAsync(newProduct);
                }
                _unitOfWork.SaveChangesAsync();

                return Response.Success("Product in Warehouse Created successfully");

            }
            catch (Exception ex)
            {
                return Response.Failure("Can't Create Product in Warehouse" + ex.Message);
            }
        }

        public bool CreationIsValid(Warehouse_ProductCreateDTO dto)
        {
            var SupplierExists = _unitOfWork.Suppliers.GetQuery().Any(s => s.Id == dto.Supplier_ID);
            var ProductExists = _unitOfWork.Products.GetQuery().Any(p => p.Code == dto.Product_Code);
            var WarehouseExists = _unitOfWork.Warehouses.GetQuery().Any(w => w.Number == dto.War_Number);

            if (!SupplierExists ||
                !ProductExists ||
                !WarehouseExists
                )
                return false;

            return true;
        }



        public Warehouse_Product? ProductExistInWarehouse(Warehouse_ProductCreateDTO dto, DateTime mfdDate, DateTime expDate)
         => _unitOfWork.WarehouseProducts.GetQuery().FirstOrDefault(wp =>
            wp.Supplier_ID == dto.Supplier_ID &&
            wp.Product_Code == dto.Product_Code &&
            wp.War_Number == dto.War_Number &&
            wp.EXP == expDate &&
            wp.MFD == mfdDate
         );

        public Warehouse_Product? ProductExistInWarehouse(int Supplier_ID, int Product_Code, int War_Number, DateTime mfdDate, DateTime expDate)
      => _unitOfWork.WarehouseProducts.GetQuery().Include(wp => wp.Product).FirstOrDefault(wp =>
         wp.Supplier_ID == Supplier_ID &&
         wp.Product_Code == Product_Code &&
         wp.War_Number == War_Number &&
         wp.EXP == expDate &&
         wp.MFD == mfdDate
      );
        public Response<Warehouse_Product> Delete(int Id)
        {
            try
            {
                var warehouseProduct = _unitOfWork.WarehouseProducts.GetQuery()
                    .Include(wp => wp.Product)
                    .FirstOrDefault(wp => wp.Id == Id);

                if (warehouseProduct == null)
                    return Response<Warehouse_Product>.Failure("Warehouse Product not found");

                // Soft delete the product
                warehouseProduct.SoftDelete(_currentUser.UserId);
                _unitOfWork.WarehouseProducts.Update(warehouseProduct);
                _unitOfWork.SaveChangesAsync();

                return Response<Warehouse_Product>.Success(warehouseProduct, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return Response<Warehouse_Product>.Failure("Can't delete Product in Warehouse" + ex.Message);
            }
        }
    }
}
