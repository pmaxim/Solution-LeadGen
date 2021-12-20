namespace BlazorWeb.Shared.Models
{
    public class TwilioWebhookViewModel
    {
        public int Id { get; set; }

        public string? DateTime { get; set; }
        public string? CallSid { get; set; }
        public string? To { get; set; }
        public string? From { get; set; }
        public string? Status { get; set; }
        public string? Direction { get; set; }
    }
}
