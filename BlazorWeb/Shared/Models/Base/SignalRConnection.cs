namespace BlazorWeb.Shared.Models.Base
{
    public abstract class SignalRConnection
    {
        public string? ConnectionId { get; set; }
        public string? UserName { get; set; }
        public string? Ip { get; set; }
    }
}
