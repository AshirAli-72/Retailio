using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("customers")]
    public class customers
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("name")]
        public string? name { get; set; }

        [Column("cnic")]
        public string? cnic { get; set; }

        [Column("contact")]
        public string? contact { get; set; }

        [Column("address")]
        public string? address { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("credit_limit")]
        public decimal? credit_limit { get; set; }     // Best as decimal

        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;
    }
}