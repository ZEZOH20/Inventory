using Inventory.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.ProductDto.Responses
{
    public class ProductResponseDTO
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}
