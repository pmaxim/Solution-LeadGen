namespace BlazorWeb.Shared.Models
{
    public class HangfireRingModel
    {
        public string? UserName { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public bool IsVonage { get; set; }
        public int Total { get; set; }
        public int Current { get; set; }
    }
}
