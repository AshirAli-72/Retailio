using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("stock_details")]
    public class StockDetail
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("item_barcode")]
        public string? item_barcode { get; set; }

        [Column("quantity")]
        public int quantity { get; set; }

        [Column("pur_price")]
        public decimal pur_price { get; set; }

        [Column("sale_price")]
        public decimal sale_price { get; set; }

        [Column("whole_sale_price")]
        public decimal whole_sale_price { get; set; }

        [Column("stock_alert")]
        public int stock_alert { get; set; }

        [Column("date_of_manafacture")]
        public string? date_of_manafacture { get; set; }

        [Column("date_of_expiry")]
        public string? date_of_expiry { get; set; }

        [Column("total_pur_price")]
        public decimal total_pur_price { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
