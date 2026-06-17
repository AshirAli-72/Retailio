using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("sale_details")]
    public class Sale
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("sale_id")]
        public int sale_id { get; set; }

        [Column("item_id")]
        public int item_id { get; set; }

        [Column("qty")]
        public int qty { get; set; }

        [Column("unit_price")]
        public decimal unit_price { get; set; }

        [Column("total_price")]
        public decimal total_price { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)PaymentStatus.Pending;

        [ForeignKey(nameof(sale_id))]
        public SaleHeader? SaleHeader { get; set; }

        [Column("business_id")]
        public int? business_id { get; set; }

        [ForeignKey("business_id")]
        public virtual Business? Business { get; set; }
    }
}
