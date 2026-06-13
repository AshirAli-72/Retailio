using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retailio.Models
{
    [Table("currencies")]
    public class Currency
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("name")]
        public string? name { get; set; }

        [Column("code")]
        public string? code { get; set; }

        [Column("symbol")]
        public string? symbol { get; set; }

        [Column("exchange_rate")]
        public decimal exchange_rate { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [Column("is_active")]
        public bool is_active { get; set; }

        [Column("user_id")]
        public int? user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual users? User { get; set; }
    }
}
