namespace BlazorWeb.Shared.Models
{
    public class LocalStorageModel
    {
        public string? Name { get; set; }
        public AccountModel Account { get; set; } = new AccountModel();
    }
}
