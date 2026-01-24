using Inventory.Data.DbContexts;
using Inventory.DTO.RO_ProductDto.Requests;
using Inventory.DTO.RO_ProductDto.Validators;
using Inventory.DTO.RO_ProductDto.Responses;
using Inventory.DTO.SO_ProductDto.Requests;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.Warehouse_ProductDto.Requests;
using Inventory.Models;
using Inventory.Services;
using Inventory.Services.CurrentUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;



namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class RO_ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly RO_ProductCreateDTOValidator _CreateDTOValidator;
        readonly IWarehouse_ProductService _Warehouse_ProductService;
        readonly ICurrentUser _currentUser;
        public RO_ProductController(
            SqlDbContext conn,
            RO_ProductCreateDTOValidator CreateDTOValidator,
            IWarehouse_ProductService Warehouse_ProductService,
            ICurrentUser currentUser
            )
        {
            _conn = conn;
            _CreateDTOValidator = CreateDTOValidator;
            _Warehouse_ProductService = Warehouse_ProductService;
            _currentUser = currentUser;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Get accessible warehouse IDs based on user role
                var accessibleWarehouseIds = await GetAccessibleWarehouseIdsAsync(_currentUser.UserId, _currentUser.UserRole);

                var products = await _conn.RO_Product
                    .Include(rop => rop.Release_Order)
                        .ThenInclude(ro => ro.Warehouse)
                    .Include(rop => rop.Release_Order)
                        .ThenInclude(ro => ro.Customer)
                    .Include(rop => rop.Product)
                    .Where(rop => accessibleWarehouseIds.Contains(rop.Release_Order.War_Number))
                    .Select(rop => new RO_ProductResponseDTO
                    {
                        Id = rop.Id,
                        RO_Amount = rop.RO_Amount,
                        RO_Unit = rop.RO_Unit,
                        RO_Price = rop.RO_Price,
                        RO_MFD = rop.RO_MFD,
                        RO_EXP = rop.RO_EXP,
                        RO_Number = rop.RO_Number,
                        Product_Code = rop.Product_Code,
                        Product = new Inventory.DTO.ProductDto.Responses.ProductResponseDTO
                        {
                            Code = rop.Product.Code,
                            Name = rop.Product.Name,
                            Unit = rop.Product.Unit,
                            Image = rop.Product.Image
                        },
                        CustomerName = rop.Release_Order.Customer.Name,
                        WarehouseName = rop.Release_Order.Warehouse.Name,
                        R_Date = rop.Release_Order.R_Date,
                        Status = rop.Release_Order.Status
                    })
                    .ToListAsync();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest("Can't get release order products" + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] RO_ProductCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            //validation
            try
            {
                // Get the warehouse product
                var warehouse_product = _conn.Warehouse_Products
                    .Include(wp => wp.Product)
                    .FirstOrDefault(wp => wp.Id == dto.WarehouseProduct_Id);
                if (warehouse_product == null)
                {
                    return BadRequest("Warehouse product not found");
                }

                // Calculate the price for the released amount
                double releasedPrice = (dto.RO_Amount / warehouse_product.Total_Amount) * warehouse_product.Total_Price;

                // Update warehouse product amounts
                warehouse_product.Total_Amount -= dto.RO_Amount;
                warehouse_product.Total_Price -= releasedPrice;

                // If total amount becomes zero or negative, soft delete it
                if (warehouse_product.Total_Amount <= 0)
                {
                    warehouse_product.SoftDelete("system"); // or get current user
                    _conn.Warehouse_Products.Update(warehouse_product);
                }

                // Add released product details into RO_Product table
                _conn.RO_Product.Add(new RO_Product
                {
                    RO_Amount = dto.RO_Amount,
                    RO_Unit = warehouse_product.Product?.Unit ?? "N/A",
                    RO_Price = releasedPrice,
                    RO_MFD = warehouse_product.MFD,
                    RO_EXP = warehouse_product.EXP,
                    RO_Number = dto.RO_Number,
                    Product_Code = warehouse_product.Product_Code
                });

                _conn.SaveChanges();

                return Ok($"Released {dto.RO_Amount} of warehouse product {dto.WarehouseProduct_Id} successfully with release order details");

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                //throw;
                return BadRequest("Can't Create Release Order" + ex.Message);
            }
        }

        void AddDeletedProductDetails(Warehouse_Product wp, int Number)
        {
            _conn.RO_Product.Add(new RO_Product
            {
                RO_Amount = wp.Total_Amount,
                RO_Unit = wp.Product?.Unit ?? "N/A", // fallback if somehow null
                RO_Price = wp.Total_Price,
                RO_Number = Number,
                Product_Code = wp.Product_Code
            });
        }

        private async Task<List<int>> GetAccessibleWarehouseIdsAsync(string userId, string userRole)
        {
            if (userRole == "Owner")
            {
                // Owners can access only warehouses they created
                return await _conn.Warehouses.Where(w => w.CreatedBy == userId).Select(w => w.Number).ToListAsync();
            }
            else if (userRole == "Manager")
            {
                // Managers can only access their assigned warehouse
                var user = await _conn.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return user?.WarehouseId.HasValue == true ? new List<int> { user.WarehouseId.Value } : new List<int>();
            }
            else
            {
                // Employees have no warehouse access
                return new List<int>();
            }
        }


    }
}
