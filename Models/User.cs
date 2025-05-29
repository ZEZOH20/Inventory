using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public enum AllowedUsers
    {
        Manager,
        Client,
        Supplier
    }
    public class User
    {
        [NotMapped]
        public AllowedUsers[] AllowedUsersArray {  get; set; }

        [EnumDataType(typeof(AllowedUsers), ErrorMessage = "Please select a valid User")]
        public string Type { get; set; }

        public int Id { get; set; }
        public required string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Mail is required")]
        public required string Mail { get; set; }

        [RegularExpression(@" ^01[0125]\d{8}$ " , ErrorMessage = "Insert a vaild Phone Number")]
        public int Phone { get; set; }
        public string? Fax {  get; set; }
        public string? Website_Domain {  get; set; }



    }
}
