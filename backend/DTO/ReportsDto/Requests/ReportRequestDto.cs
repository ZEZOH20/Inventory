using System.ComponentModel.DataAnnotations;

namespace Inventory.DTO.ReportsDto.Requests
{
    public class ReportRequestDto
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? WarehouseId { get; set; }

        // Validation: End date should be after start date
        public bool IsValidDateRange()
        {
            return EndDate >= StartDate;
        }
    }
}