namespace BlazorWeb.Shared.Models
{
    public class AccountNumbersModel
    {
        public int Id { get; set; }
        public string? FromNexmo { get; set; }
        public object? FromTwilio { get; set; }
        public string? Sid { get; set; }
        public string? Token { get; set; }
        public bool IsVonage { get; set; }
    }
}
