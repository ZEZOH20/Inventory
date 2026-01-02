using Inventory.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.DTO.WarehouseDto.Requests
{
    public class WarehouseCreateDTO
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ManagerId { get; set; }
    }
}
