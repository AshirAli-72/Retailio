using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
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

        [Column("payment_method")]
        public int? payment_method { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)PaymentStatus.Pending;

        [Column("file")]
        public string? file { get; set; }

        [Column("business_id")]
        public int? business_id { get; set; }

        [ForeignKey("business_id")]
        public virtual users? User { get; set; }
    }
}
