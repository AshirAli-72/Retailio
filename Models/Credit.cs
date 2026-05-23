using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("credits")]
    public class Credit
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("inv_no")]
        public string? inv_no { get; set; }

        [Column("customer_id")]
        public int customer_id { get; set; }

        [Column("grand_total")]
        public decimal grand_total { get; set; }

        [Column("paid_amount")]
        public decimal paid_amount { get; set; }

        [Column("discount")]
        public decimal discount { get; set; }

        [Column("remaining_amount")]
        public decimal remaining_amount { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    }
}
