using Inventory.Models;
using Microsoft.AspNetCore.Http;

namespace Inventory.DTO.ProductDto.Requests
{
    public class ProductCreateDTO
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
