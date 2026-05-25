using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("sale_details")]
    public class Sale
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("sale_id")]
        public int sale_id { get; set; }

        [Column("item_id")]
        public int item_id { get; set; }

        [Column("qty")]
        public int qty { get; set; }

        [Column("unit_price")]
        public decimal unit_price { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [ForeignKey(nameof(sale_id))]
        public SaleHeader? SaleHeader { get; set; }
    }
}
