namespace BlazorWeb.Server.Models
{
    public class RequestResponseHeader
    {
        public List<string> RequestHeaders { get; set; } = new List<string>();
        public List<string> ResponseHeaders { get; set; } = new List<string>();
    }
}
