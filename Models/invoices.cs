using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("invoices")]
    public class invoices
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("invoice_no")]
        public string? invoice_no { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("seller_name")]
        public string? seller_name { get; set; }

        [Column("seller_contact")]
        public string? seller_contact { get; set; }

        [Column("seller_address")]
        public string? seller_address { get; set; }

        [Column("customer_name")]
        public string? customer_name { get; set; }

        [Column("customer_address")]
        public string? customer_address { get; set; }

        [Column("customer_contact")]
        public string? customer_contact { get; set; }
        [Column("prod_name/service")]
        public string? prod_name_service { get; set; }

        [Column("qty/unit_type")]
        public string? qty_unit_type { get; set; }

        [Column("price")]
        public decimal price { get; set; }

        [Column("discount")]
        public decimal discount { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }


        [Column("payment")]
        public string? payment { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

    }
}
