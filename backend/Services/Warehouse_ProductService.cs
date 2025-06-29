using Inventory.Data.DbContexts;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public interface IWarehouse_ProductService{
        string CreateWarehouse_Product(Warehouse_ProductCreateDTO dto);
        bool CreationIsValid(Warehouse_ProductCreateDTO dto);
         Warehouse_Product? ProductExistInWarehouse(Warehouse_ProductCreateDTO dto, DateTime mfdDate, DateTime expDate);
        Warehouse_Product? ProductExistInWarehouse(int Supplier_ID, int Product_Code, int War_Number, DateTime mfdDate, DateTime expDate);
        Warehouse_Product Delete(int Id);
    }
    public class Warehouse_ProductService : IWarehouse_ProductService
    {
        readonly SqlDbContext _conn;
        public Warehouse_ProductService(SqlDbContext conn)
        {
            _conn = conn;
        }

       
        public string CreateWarehouse_Product(Warehouse_ProductCreateDTO dto)
        {

            //validation
            if (!dto.Valid())
                return $"check: \n" +
                    $"Product_Code,\n" +
                    $"War_Number,\n" +
                    $"Amount,\n" +
                    $"Price,\n" +
                    $"greater than > 0 \n\n" +
                    $"MFD , EXP , EXP are Date Type";

            if (!CreationIsValid(dto))
                return $"Supplier_ID or \n " +
                    $"Product_Code or \n " +
                    $"Warehouse_Number \n  " +
                    "can't found check them and try later";

            // Parse and validate dates
            DateTime mfdDate = DateTime.Parse(dto.MFD);
            DateTime expDate = DateTime.Parse(dto.EXP);
            if (expDate <= mfdDate)
                return $"EXP Date : {expDate} \n " +
                    $"can't be less than or equal\n" +
                    $"MFD Date : {mfdDate}";
            //validation

            try
            {
                var existingProduct = ProductExistInWarehouse(dto, mfdDate , expDate);
                //var existingProduct = ProductExistInWarehouse(dto.Supplier_ID, dto.Product_Code, dto.War_Number, mfdDate , expDate);

                if (existingProduct is not null)
                {
                    existingProduct.Total_Amount += dto.Amount;
                    existingProduct.Total_Price += dto.Amount * dto.Price;
                }
                else
                {


                    _conn.Warehouse_Products.Add(new Warehouse_Product
                    {
                        War_Number = dto.War_Number,
                        Product_Code = dto.Product_Code,
                        Supplier_ID = dto.Supplier_ID,
                        MFD = mfdDate,
                        EXP = expDate,
                        Store_Date = DateTime.UtcNow,
                        Total_Amount = dto.Amount,
                        Total_Price = dto.Amount * dto.Price,
                    });
                }
                _conn.SaveChanges();

                return "Product in Warehouse Created successfully";

            }
            catch (Exception ex)
            {
                return "Can't Create Product in Warehouse" + ex.Message;
            }
        }

        public bool CreationIsValid(Warehouse_ProductCreateDTO dto)
        {
            var SupplierExists = _conn.Suppliers.Any(s => s.Id == dto.Supplier_ID);
            var ProductExists = _conn.Products.Any(p => p.Code == dto.Product_Code);
            var WarehouseExists = _conn.Warehouses.Any(w => w.Number == dto.War_Number);

            if (!SupplierExists ||
                !ProductExists ||
                !WarehouseExists
                )
                return false;

            return true;
        }



        public Warehouse_Product? ProductExistInWarehouse(Warehouse_ProductCreateDTO dto, DateTime mfdDate, DateTime expDate)
         => _conn.Warehouse_Products.FirstOrDefault(wp =>
            wp.Supplier_ID == dto.Supplier_ID &&
            wp.Product_Code == dto.Product_Code &&
            wp.War_Number == dto.War_Number &&
            wp.EXP == expDate &&
            wp.MFD == mfdDate
         );

        public Warehouse_Product? ProductExistInWarehouse(int Supplier_ID, int Product_Code, int War_Number, DateTime mfdDate, DateTime expDate)
      => _conn.Warehouse_Products.Include(wp=>wp.Product).FirstOrDefault(wp =>
         wp.Supplier_ID == Supplier_ID &&
         wp.Product_Code == Product_Code &&
         wp.War_Number == War_Number &&
         wp.EXP == expDate &&
         wp.MFD == mfdDate
      );
        public Warehouse_Product Delete(int Id)
        {
     
            try
            {


                var Warehouse_Product = _conn.Warehouse_Products
                    .Include(wp => wp.Product) // Include navigation property
                    .FirstOrDefault(wp => wp.Id == Id);
                if(Warehouse_Product == null)
                    throw new Exception();

                // Create a copy BEFORE deleting it
                var warehouseProductCopy = new Warehouse_Product
                {
                    Id = Warehouse_Product.Id,
                    War_Number = Warehouse_Product.War_Number,
                    Product_Code = Warehouse_Product.Product_Code,
                    Supplier_ID = Warehouse_Product.Supplier_ID,
                    MFD = Warehouse_Product.MFD,
                    EXP = Warehouse_Product.EXP,
                    Store_Date = Warehouse_Product.Store_Date,
                    Total_Amount = Warehouse_Product.Total_Amount,
                    Total_Price = Warehouse_Product.Total_Price,
                    Product = Warehouse_Product.Product // assign the Product reference too
                };

                _conn.Warehouse_Products.Remove(Warehouse_Product);
                _conn.SaveChanges();

                return warehouseProductCopy;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't delete Product in Warehouse" + ex.Message);
            }
        }
    }
}
