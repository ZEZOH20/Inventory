using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Person : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Insert a vaild Phone Number")]
        public int Phone { get; set; }
        public string? Fax { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Mail is required")]
        public string Mail { get; set; }
        public string? Domain { get; set; }
    }
}
