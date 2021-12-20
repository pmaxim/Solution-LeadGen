namespace BlazorWeb.Shared.Models
{
    public class ScheduleViewModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public bool IsAveryDay { get; set; }
    }
}
