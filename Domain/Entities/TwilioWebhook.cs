using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    //https://www.twilio.com/docs/voice/api/call-resource#statuscallback
    public class TwilioWebhook
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(34)]
        public string CallSid { get; set; }

        [Required]
        [StringLength(14)]
        public string To { get; set; }

        [Required]
        [StringLength(14)]
        public string From { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [StringLength(20)]
        public string Direction { get; set; }
    }
}
