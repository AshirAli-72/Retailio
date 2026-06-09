using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_Invoice_system.Services;

namespace E_Invoice_system.Models
{
    [Table("return_details")]
    public class ReturnDetail
    {
        [Key]
        [Column("id", Order = 0)]
        public int Id { get; set; }

        [Column("inv_no", Order = 1)]
        public string? inv_no { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("sale_id")]
        public int? sale_id { get; set; }

        [Column("item_id")]
        public int item_id { get; set; }

        [Column("qty")]
        public int qty { get; set; }

        [Column("unit_price")]
        public decimal unit_price { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }

        [Column("payment_method")]
        public int? payment_method { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)PaymentStatus.Returned;
    }
}
