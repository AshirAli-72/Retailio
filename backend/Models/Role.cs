using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("role_title")]
        [MaxLength(100)]
        public string RoleTitle { get; set; } = string.Empty;
    }
}
