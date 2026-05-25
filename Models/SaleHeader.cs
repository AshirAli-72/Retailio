using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("sales")]
    public class SaleHeader
    {
        [Key]
        [Column("id", Order = 0)]
        public int id { get; set; }

        [Column("inv_no", Order = 1)]
        public string? inv_no { get; set; }

        [Column("customer_id")]
        public int? customer_id { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("gross_total")]
        public decimal gross_total { get; set; }

        [Column("discount")]
        public decimal discount { get; set; }

        [Column("net_payable")]
        public decimal net_payable { get; set; }

        [Column("paid")]
        public decimal paid { get; set; }

        [Column("due")]
        public decimal due { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [Column("payment_method")]
        public string? payment_method { get; set; }

        public ICollection<Sale> SaleDetails { get; set; } = new List<Sale>();
    }
}
