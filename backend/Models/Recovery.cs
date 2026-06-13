using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("recoveries")]
    public class Recovery
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("customer_id")]
        public int customer_id { get; set; }

        [Column("credit_id")]
        public int? credit_id { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("total_credit")]
        public decimal total_credit { get; set; }


        [Column("paid")]
        public decimal paid { get; set; }

        [Column("remaining")]
        public decimal remaining { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)PaymentStatus.Pending;

        [Column("file")]
        public string? file { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
