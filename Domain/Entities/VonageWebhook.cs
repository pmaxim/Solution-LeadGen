using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class VonageWebhook
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(14)]
        public string From { get; set; }

        [Required]
        [StringLength(14)]
        public string To { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public decimal Price { get; set; }

        public decimal Rate { get; set; }

        public Guid Uuid { get; set; }
    }
}
