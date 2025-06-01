using System.Numerics;

namespace Inventory.DTO.ProductDto.Requests
{
    public class ProductUpdateDTO
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        //public string? Unit { get; set; }

        // Custom validation to ensure at least one field is provided
        public bool HasAtLeastOneValue()
        {
            return !string.IsNullOrEmpty(Name);
                   //|| !string.IsNullOrEmpty(Unit)
        }
    }
}
