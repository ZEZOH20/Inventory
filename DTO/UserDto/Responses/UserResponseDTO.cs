using Inventory.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.UserDto.Responses
{
    public class UserResponseDTO
    {
        public UserResponseDTO(User user)
        {
            Id = user.Id;   
            Name = user.Name;
            Phone = user.Phone;
            Fax = user.Fax ?? "No Fax";  
            Mail = user.Mail;
            Domain = user.Domain ?? "No Domain";  
        }
        public int Id { get; set; }
        public  string Name { get; set; }
        public int Phone { get; set; }
        public string Fax { get; set; } = "No Fax";
        public  string Mail { get; set; }
        public string Domain { get; set; } = "No Domain";
    }
}
