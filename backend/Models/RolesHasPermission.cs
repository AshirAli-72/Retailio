using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("roles_has_permissions")]
    public class RolesHasPermission
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("permission_id")]
        public int PermissionId { get; set; }

        [Column("business_id")]
        public int? business_id { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission? Permission { get; set; }

        [ForeignKey("business_id")]
        public virtual Business? Business { get; set; }
    }
}
