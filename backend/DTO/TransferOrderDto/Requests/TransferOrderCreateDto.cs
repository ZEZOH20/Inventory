using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.DTO.TransferOrderDto.Requests
{
    public class TransferOrderCreateDto
    {
        public int Supplier_ID { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
