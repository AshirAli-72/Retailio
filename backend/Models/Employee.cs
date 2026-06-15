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

        [Column("date")]
        public string date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Required(ErrorMessage = "Full Name is required")]
        [Column("full_name")]
        public string? full_name { get; set; }

        [Column("cnic")]
        public string? cnic { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Column("email")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [Column("mobile_no")]
        public string? mobile_no { get; set; }

        [Required(ErrorMessage = "Residential Address is required")]
        [Column("address")]
        public string? address { get; set; }

        [Column("image_path")]
        public string? image_path { get; set; }

        [Column("salary")]
        public decimal salary { get; set; }

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;

        /// <summary>Hashed login password — mirrors users.password</summary>
        [Column("password")]
        public string? password { get; set; }

        /// <summary>FK to roles — mirrors users.role_id</summary>
        [Column("role_id")]
        public int? role_id { get; set; }

        /// <summary>FK to users — the system account created for this employee</summary>
        [Column("business_id")]
        public int? business_id { get; set; }
    }
}
