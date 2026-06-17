using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Retailio.Services;

namespace Retailio.Models
{
    [Table("subscriptions")]
    public class Subscription
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("business_id")]
        public int business_id { get; set; }

        /// <summary>free_trial | basic | professional | enterprise</summary>
        [Column("plan")]
        public string? plan { get; set; }

        /// <summary>Date string in yyyy-MM-dd format e.g. 2026-06-13</summary>
        [Column("started_at")]
        public string? started_at { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

        /// <summary>Date string in yyyy-MM-dd format e.g. 2026-06-27 — null for paid plans</summary>
        [Column("expires_at")]
        public string? expires_at { get; set; }

        /// <summary>1 = Active, 2 = Inactive — mirrors EntityStatus enum</summary>
        [Column("status")]
        public int? status { get; set; } = (int)EntityStatus.Active;

        [ForeignKey("business_id")]
        public virtual Business? Business { get; set; }
    }
}
