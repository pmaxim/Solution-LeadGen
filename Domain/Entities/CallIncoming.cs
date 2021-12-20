using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CallIncoming
    {
        [Key]
        public int Id { get; set; }
        public float Price { get; set; }
        public int Duration { get; set; } //The length of the call in seconds.
        public int Count { get; set; }
        public string To { get; set; }
        public virtual DashboardIncomingCall DashboardIncomingCall { get; set; }
    }
}
