using Newtonsoft.Json;

namespace BlazorWeb.Server.Models
{
    //https://developer.vonage.com/voice/voice-api/webhook-reference#event-webhook
    public class VonageVoiceEndPoint
    {
        [JsonProperty("duration")]
        public int? Duration { get; set; }
        [JsonProperty("start_time")]
        public DateTime? StartTime { get; set; }
        [JsonProperty("rate")]
        public decimal? Rate { get; set; }
        [JsonProperty("price")]
        public decimal? Price { get; set; }
        [JsonProperty("end_time")]
        public DateTime? EndTime { get; set; }
        [JsonProperty("from")]
        public string? From { get; set; }
        [JsonProperty("to")]
        public string? To { get; set; }
        [JsonProperty("uuid")]
        public Guid? Uuid { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("direction")]
        public string? Direction { get; set; }
        [JsonProperty("network")]
        public string? Network { get; set; }
    }
}
