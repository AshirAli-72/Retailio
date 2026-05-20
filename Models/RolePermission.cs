using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Invoice_system.Models
{
    [Table("roles_permissions")]
    public class RolePermission
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("dashboard")]
        public bool Dashboard { get; set; }

        [Column("customers")]
        public bool Customers { get; set; }

        [Column("products")]
        public bool Products { get; set; }

        [Column("sales")]
        public bool Sales { get; set; }

        [Column("employees")]
        public bool Employees { get; set; }

        [Column("Reports")]
        public bool Reports { get; set; }

        [Column("settings")]
        public bool Settings { get; set; }

        [Column("inventory")]
        public bool Inventory { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}
