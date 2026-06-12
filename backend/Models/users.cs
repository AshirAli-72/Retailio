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

        [Column("email")]
        public string? email { get; set; }

        [Column("username")]
        public string? username { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("role_id")]
        public int role_id { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;

        [ForeignKey("role_id")]
        public virtual Role? Role { get; set; }
    }
}
