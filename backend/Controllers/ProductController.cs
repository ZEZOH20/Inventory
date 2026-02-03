using Inventory.Models;
using Microsoft.AspNetCore.Mvc;
using Inventory.DTO.UserDto.Responses;
using Inventory.Data.DbContexts;
using Inventory.DTO.WarehouseDto.Requests;
using Inventory.DTO.WarehouseDto.Validations;
using Microsoft.EntityFrameworkCore;
using Inventory.DTO.WarehouseDto.Responses;
using Inventory.DTO.ProductDto.Responses;
using Inventory.DTO.ProductDto.Requests;
using System.Drawing;
using System.IO.Pipelines;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using Inventory.Services;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(Roles = "Owner,Manager")]
    public class ProductController : ControllerBase
    {
        readonly SqlDbContext _conn;
        readonly IImageService _imageService;
        public ProductController(SqlDbContext conn, IImageService imageService)
        {
            _conn = conn;
            _imageService = imageService;
        }
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var Products = _conn.Products
                    .Select(p => new ProductResponseDTO
                    {
                        Code = p.Code,
                        Name = p.Name,
                        Unit = p.Unit,
                        Image = p.Image
                    })
                    .ToList();

                return Ok(Products);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't return Products" + ex.Message);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
        {

            try
            {
                string? imagePath = null;
                if (dto.ImageFile != null)
                {
                    imagePath = await _imageService.UploadImageAsync(dto.ImageFile, "products");
                }

                _conn.Products.Add(new Product
                {
                    Name = dto.Name,
                    Unit = dto.Unit,   // I will display all units to choose between them  
                    Image = imagePath
                });
                _conn.SaveChanges();

                return Ok("Product Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create Product" + ex.Message);
            }
        }

        [HttpGet("AllSystemUnits")]
        public IActionResult AllSystemUnits()
        {
            Dictionary<WeightUnit, double> units = new Dictionary<WeightUnit, double>
{
                { WeightUnit.Kilogram, 20.0 },
                { WeightUnit.Ton, 1000.0 },
                { WeightUnit.Pound, 0.45 },
                { WeightUnit.Pack, 1.0 },
                { WeightUnit.Dozen, 12.0 },
                { WeightUnit.Liter, 1.0 },
                { WeightUnit.Piece, 1.0 }
            };

            return Ok(units);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateBYId([FromForm] ProductUpdateDTO dto)
        {
            var validationResult = dto.Code > 0;

            if (!validationResult)
                return BadRequest($"your code {dto.Code} should be positve value");

            try
            {
                var result = await UpdateProduct(dto);

                return !result ?
                     NotFound($"Product Code:  {dto.Code} not found") :
                     Ok("Product updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Update Warehouse" + ex.Message);
            }
        }

        [HttpDelete("delete/{code}")]
        public async Task<IActionResult> Delete(int code)
        {
            //validate ID
            if (code <= 0)
                return BadRequest($"the Product code: {code} can't be zero and should be positive ");

            try
            {
                var Product = _conn.Products.FirstOrDefault(p => p.Code == code);

                if (Product == null)
                    return BadRequest($"the Product code {code} not found ");

                // Delete associated image if exists
                if (!string.IsNullOrEmpty(Product.Image))
                {
                    await _imageService.DeleteImageAsync(Product.Image);
                }

                _conn.Products.Remove(Product);
                _conn.SaveChanges();

                return Ok("Product deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Can't delete Product" + ex.Message);
            }
        }

        async Task<bool> UpdateProduct(ProductUpdateDTO dto)
        {
            var Product = _conn.Products.FirstOrDefault(p => p.Code == dto.Code);

            if (Product == null)
                return false;

            // Update only provided values
            if (!string.IsNullOrEmpty(dto.Name))
                Product.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Unit))
                Product.Unit = dto.Unit;

            if (dto.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(Product.Image))
                {
                    await _imageService.DeleteImageAsync(Product.Image);
                }
                // Upload new image
                Product.Image = await _imageService.UploadImageAsync(dto.ImageFile, "products");
            }

            _conn.SaveChanges();

            return true;
        }
    }
}
