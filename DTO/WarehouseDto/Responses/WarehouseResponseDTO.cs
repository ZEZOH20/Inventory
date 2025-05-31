using Inventory.DTO.UserDto.Responses;

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
    }
}
