using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("sale_details")]
    public class Sale
    {
        [Key]
        [Column("id", Order = 0)]
        public int id { get; set; }

        [Column("billNo", Order = 1)]
        public string? billNo { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
   
        [Column("no_of_items")]
        public int no_of_items { get; set; }

        [Column("qty")]
        public decimal qty { get; set; }

        [Column("total_qty")]
        public decimal total_qty { get; set; }

        [Column("price")]
        public decimal price { get; set; }

        [Column("discount")]
        public decimal discount { get; set; }
        [Column("expiry_date")]
        public string? expiry_date { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }
        [Column("description")]
        public string? description { get; set; }

        [Column("payment_method")]
        public string? payment_method { get; set; }
        [Column("status")]
        public string? status { get; set; }

        [Column("is_returned")]
        public bool is_returned { get; set; } = false;

        [Column("user_id")]
        public int? user_id { get; set; }

    }
}
