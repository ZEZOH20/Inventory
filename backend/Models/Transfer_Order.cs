using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models
{
    public class Transfer_Order : AuditableEntity
    {
        [Key]
        public int Number { get; set; }

        [ForeignKey("Supplier")]
        public required int Supplier_ID { get; set; }

        [ForeignKey("FromWarehouse")]
        public required int From { get; set; }

        [ForeignKey("ToWarehouse")]
        public required int To { get; set; }

        [DataType(DataType.Date)]
        public DateTime T_Date { get; set; }

        //Navigation
        public Supplier Supplier { get; set; }
        public Warehouse FromWarehouse { get; set; }
        public Warehouse ToWarehouse { get; set; }
        public List<TO_Product> TO_Products { get; set; }
    }
}



//Enum convertingBetweenUnits
//{
//    1 ton => 1000 kg
//}

//SO_Product 
//    code    amount  unit    unit_price
//    020     2       ton     10000

//Warehouse_Product
//    Amount              price
//    SO_Amountx1000      SO_Price/1000

//Product
//    code    unit
//    020     kg
