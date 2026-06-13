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

        [Column("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
