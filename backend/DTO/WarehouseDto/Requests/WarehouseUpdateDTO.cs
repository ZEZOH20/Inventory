using Microsoft.IdentityModel.Tokens;
using System.Numerics;

namespace Inventory.DTO.WarehouseDto.Requests
{
    public class WarehouseUpdateDTO
    {
        public int Number {  get; set; }
        public string? Name { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? ManagerId { get; set; }

        // Custom validation to ensure at least one field is provided
        public bool HasAtLeastOneValue()
        {
            return !string.IsNullOrEmpty(Name) ||
                   !string.IsNullOrEmpty(Region) ||
                   !string.IsNullOrEmpty(City) ||
                   !string.IsNullOrEmpty(Street) ||
                   !string.IsNullOrEmpty(ManagerId);
        }
    }
}
