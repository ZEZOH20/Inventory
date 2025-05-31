using Inventory.DTO.ProductDto.Responses;
using Inventory.DTO.UserDto.Responses;
using Inventory.DTO.Warehouse_ProductDto.Responses;

namespace Inventory.DTO.WarehouseDto.Responses
{
    public class WarehouseResponseDTO
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public UserResponseDTO Manager { get; set; }
        public List<Warehouse_ProductResponseDTO> Warehouse_Products { get; set; }

        
    }
}
