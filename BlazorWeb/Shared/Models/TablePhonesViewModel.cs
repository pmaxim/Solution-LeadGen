namespace BlazorWeb.Shared.Models
{
    public class TablePhonesViewModel
    {
        public List<TablePhoneItem> List { get; set; } = new List<TablePhoneItem>();
    }

    public class TablePhoneItem
    {
        public int CountNoCall { get; set; }
        public int CountIsCall { get; set; }
        public string? Title { get; set; }
    }
}
