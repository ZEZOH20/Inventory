using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Inventory.Models;
using Inventory.DTO.SO_ProductDto;

namespace Inventory.DTO.SupplyOrderDto.Requests
{
    public class SupplyOrderCreateDTO
    {
        public  int Supplier_ID { get; set; }
        public  int War_Number { get; set; }
        //public List<SO_ProductCreateDTO> Products {  get; set; }
    }
}
