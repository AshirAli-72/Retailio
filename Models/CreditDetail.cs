using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("credits_details")]
    public class CreditDetail
    {
        [Key]
        [Column("id", Order = 0)]
        public int id { get; set; }

        [Column("credit_id")]
        public int credit_id { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("no_of_items")]
        public int no_of_items { get; set; }

        [Column("qty")]
        public int qty { get; set; }

        [Column("total_qty")]
        public int total_qty { get; set; }

        [Column("price")]
        public decimal price { get; set; }

        [Column("discount")]
        public decimal discount { get; set; }

        [Column("expiry_date")]
        public string? expiry_date { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }

        [Column("payment_method")]
        public string? payment_method { get; set; } = "Credit";

        [Column("status")]
        public string? status { get; set; } = "Pending";

        [Column("paid_amount")]
        public decimal paid_amount { get; set; }

        [Column("remaining_amount")]
        public decimal remaining_amount { get; set; }

    }
}
