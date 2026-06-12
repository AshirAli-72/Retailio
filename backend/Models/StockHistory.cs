using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("stock_history")]
    public class stock_history
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("date")]
        public string? date { get; set; }

        [Column("new_quantity")]
        public int new_quantity { get; set; }

        [Column("old_quantity")]
        public int old_quantity { get; set; }

        [Column("new_purchase_price")]
        public decimal new_purchase_price { get; set; }

        [Column("old_purchase_price")]
        public decimal old_purchase_price { get; set; }

        [Column("new_sale_price")]
        public decimal new_sale_price { get; set; }

        [Column("old_sale_price")]
        public decimal old_sale_price { get; set; }

        [Column("remarks")]
        public string? remarks { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [Column("product_id")]
        public int product_id { get; set; }
    }
}
