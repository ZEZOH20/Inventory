using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [RegularExpression(@" ^01[0125]\d{8}$ ", ErrorMessage = "Insert a vaild Phone Number")]
        public int Phone { get; set; }

        public string? Fax { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Mail is required")]
        public required string Mail { get; set; }
        public string? Domain {  get; set; }

    }
}
