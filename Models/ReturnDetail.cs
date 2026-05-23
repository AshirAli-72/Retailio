using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("sale_id")]
        public int SaleId { get; set; }

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Column("no_of_items")]
        public int no_of_items { get; set; }

        [Column("qty")]
        public int qty { get; set; }

        [Column("total_qty")]
        public int total_qty { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("method")]
        public string? Method { get; set; }

        [Column("status")]
        public string? Status { get; set; }
    }
}
