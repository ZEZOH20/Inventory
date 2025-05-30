using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.UserDto.Requests
{
    public class UserUpdateDTO
    {

        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Mail { get; set; }
        public string? Domain { get; set; }

        // Custom validation to ensure at least one field is provided
        public bool HasAtLeastOneValue()
        {
            return !string.IsNullOrEmpty(Name) ||
                   !string.IsNullOrEmpty(Phone) ||
                   !string.IsNullOrEmpty(Fax) ||
                   !string.IsNullOrEmpty(Mail) ||
                   !string.IsNullOrEmpty(Domain);
        }
    }
}
