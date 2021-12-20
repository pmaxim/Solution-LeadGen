namespace BlazorWeb.Shared.Models
{
    public class DashboardIncomingCallsViewModel
    {
        public List<DashboardIncomingCallsItem> List { get; set; } = new List<DashboardIncomingCallsItem>();
    }

    public class DashboardIncomingCallsItem
    {
        public int Id { get; set; }
        public string Price { get; set; }
        public int Duration { get; set; }
        public int Count { get; set; }
        public string? To { get; set; }
        public string? Date { get; set; }
    }
}
