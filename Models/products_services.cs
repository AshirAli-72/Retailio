using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_Invoice_system.Services;

namespace E_Invoice_system.Models
{
    [Table("products_services")]
    public class ProductService
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("prod_name")]
        public string? prod_name { get; set; }

        [Column("barcode")]
        public string? barcode { get; set; }

        [Column("manufacture_date")]
        public string? manufacture_date { get; set; }

        [Column("expiry_date")]
        public string? expiry_date { get; set; }

        [Column("prod_state")]
        public string? prod_state { get; set; }

        [Column("unit")]
        public string? unit { get; set; }

        [Column("item_type")]
        public string? item_type { get; set; }

        [Column("size")]
        public int? size { get; set; }

        [Column("pic")]
        public string? pic { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;

        [Column("remarks")]
        public string? remarks { get; set; }

        [Column("category_id")]
        public int? category_id { get; set; }

        [Column("brand_id")]
        public int? brand_id { get; set; }

        [NotMapped]
        public int StockQty { get; set; }
    }
}
