using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("store_configurations")]
    public class StoreConfiguration
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("shop_name")]
        [MaxLength(200)]
        public string? ShopName { get; set; }

        [Column("owner_name")]
        [MaxLength(200)]
        public string? OwnerName { get; set; }

        [Column("city")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Column("address")]
        [MaxLength(500)]
        public string? Address { get; set; }

        [Column("business_nature")]
        [MaxLength(200)]
        public string? BusinessNature { get; set; }

        [Column("branch")]
        [MaxLength(100)]
        public string? Branch { get; set; }

        [Column("shop_no")]
        [MaxLength(50)]
        public string? ShopNo { get; set; }

        [Column("phone_1")]
        [MaxLength(50)]
        public string? Phone1 { get; set; }

        [Column("phone_2")]
        [MaxLength(50)]
        public string? Phone2 { get; set; }

        [Column("logo_path")]
        [MaxLength(500)]
        public string? LogoPath { get; set; }

        [Column("comments")]
        public string? Comments { get; set; }
    }
}
