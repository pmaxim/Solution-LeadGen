namespace BlazorWeb.Shared.Models
{
    public class FilterDashboardModel
    {
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
