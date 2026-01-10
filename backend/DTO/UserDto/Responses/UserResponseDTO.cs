using Inventory.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.UserDto.Responses
{
    public class UserResponseDTO
    {
        public UserResponseDTO() { }
        public UserResponseDTO(ApplicationUser user)
        {
            Id = user.Id;
            Name = user.Name;
            Phone = user.PhoneNumber;
            Fax = "No Fax";  // ApplicationUser doesn't have Fax
            Mail = user.Email;
        }
        public UserResponseDTO(Customer user)
        {
            Id = user.Id.ToString();
            Name = user.Name;
            Phone = user.Phone.ToString();
            Fax = user.Fax ?? "No Fax";
            Mail = user.Mail;
        }
        public UserResponseDTO(Supplier user)
        {
            Id = user.Id.ToString();
            Name = user.Name;
            Phone = user.Phone.ToString();
            Fax = user.Fax ?? "No Fax";
            Mail = user.Mail;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; } = "No Fax";
        public string Mail { get; set; }
    }
}

