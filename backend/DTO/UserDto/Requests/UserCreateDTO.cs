using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.UserDto.Requests
{
    public class UserCreateDTO
    {
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public string? Fax { get; set; }
        public required string Mail { get; set; }
        public string? Domain { get; set; }
    }
}
