using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("users")]
    public class users
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("business_id")]
        public int business_id { get; set; }

        [Column("name")]
        public string? name { get; set; }

        [Column("contact")]
        public string? contact { get; set; }

        [Column("cnic")]
        public string? cnic { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;
    }
}
