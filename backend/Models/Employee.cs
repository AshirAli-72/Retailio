using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("employee")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("business_id")]
        public int? business_id { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Column("name")]
        public string? name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Column("email")]
        public string? email { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("cnic")]
        public string? cnic { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        [Column("contact")]
        public string? contact { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Column("address")]
        public string? address { get; set; }

        [Column("image_path")]
        public string? image_path { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;
    }
}
