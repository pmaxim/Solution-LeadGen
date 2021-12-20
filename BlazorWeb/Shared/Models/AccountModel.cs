using BlazorWeb.Shared.Models.Base;

namespace BlazorWeb.Shared.Models
{
    public class AccountModel : SignalRConnection
    {
        public int Id { get; set; }
        public string? Sid { get; set; }
        public string? Token { get; set; }
        public string? Title { get; set; }
        public bool IsVonage { get; set; }
        public bool IsActive { get; set; }
        public List<AccountApplicationModel> List { get; set; } = new List<AccountApplicationModel>();
        public string? Provider { get; set; }
    }

    public class AccountApplicationModel : VonageApplication
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
