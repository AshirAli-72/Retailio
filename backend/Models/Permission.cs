using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("permissions")]
    public class Permission
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("names")]
        public string? Names { get; set; }

        [Column("slugs")]
        public string? Slugs { get; set; }


    }
}
