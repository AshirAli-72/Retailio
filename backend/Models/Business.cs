using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("businesses")]
    public class Business
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }

        [Column("business_name")]
        public string? business_name { get; set; }

        [Column("business_type")]
        public string? business_type { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
