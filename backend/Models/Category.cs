using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Required]
        [Column("category_title")]
        [MaxLength(200)]
        public string category_title { get; set; } = string.Empty;

        [Column("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
