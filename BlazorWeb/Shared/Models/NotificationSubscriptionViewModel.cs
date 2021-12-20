namespace BlazorWeb.Shared.Models
{
    public class NotificationSubscriptionViewModel
    {
        public int NotificationSubscriptionId { get; set; }

        public string? UserId { get; set; }

        public string? Url { get; set; }

        public string? P256dh { get; set; }

        public string? Auth { get; set; }
    }
}
