using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("user_has_roles")]
    public class UserHasRoles
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("roles_has_permission_id")]
        public int RolesHasPermissionId { get; set; }

        [Column("business_id")]
        public int? business_id { get; set; }

        [ForeignKey("UserId")]
        public virtual users? User { get; set; }

        [ForeignKey("RolesHasPermissionId")]
        public virtual RolesHasPermission? RolesHasPermission { get; set; }

        [ForeignKey("business_id")]
        public virtual Business? Business { get; set; }
    }
}
