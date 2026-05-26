using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("credits_details")]
    public class CreditDetail
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("credit_id")]
        public int credit_id { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("sale_id")]
        public int sale_id { get; set; }

        [Column("amount")]
        public decimal amount { get; set; }

        [Column("paid")]
        public decimal paid { get; set; }

        [Column("due")]
        public decimal due { get; set; }

        [Column("payment_method")]
        public string? payment_method { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [Column("file")]
        public string? file { get; set; }
    }
}
