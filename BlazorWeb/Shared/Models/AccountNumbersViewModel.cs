namespace BlazorWeb.Shared.Models
{
    public class AccountNumbersViewModel
    {
        public List<AccountNumbersItem> List { get; set; } = new List<AccountNumbersItem>();
    }

    public class AccountNumbersItem
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public string? Sid { get; set; }
        public string? Token { get; set; }
        public bool IsVonage { get; set; }
    }
}
