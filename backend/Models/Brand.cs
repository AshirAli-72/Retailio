using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("brands")]
    public class Brand
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Required]
        [Column("brand_title")]
        [MaxLength(200)]
        public string brand_title { get; set; } = string.Empty;
    }
}
