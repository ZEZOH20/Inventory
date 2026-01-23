using System.ComponentModel.DataAnnotations;

namespace Inventory.Models
{
    public class Person : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Phone { get; set; }
        public string? Fax { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Mail is required")]
        public string Mail { get; set; }
        public string? Domain { get; set; }
    }
}
