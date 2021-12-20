namespace BlazorWeb.Server.Models
{
    //https://www.twilio.com/docs/voice/api/call-resource#statuscallbackevent
    //https://www.twilio.com/docs/voice/api/call-resource#recordingstatuscallback
    //StatusCallbackEvent
    public class TwilioStatus
    {
        public string? Called { get; set; }
        public string? ToState { get; set; }
        public string? CallerCountry { get; set; }
        public string? Direction { get; set; }
        public string? Timestamp { get; set; }
        public string? CallbackSource { get; set; }

        public string? CallerState { get; set; }
        public string? ToZip { get; set; }
        public string? SequenceNumber { get; set; }
        public string? CallSid { get; set; }
        public string? To { get; set; }
        public string? CallerZip { get; set; }

        public string? ToCountry { get; set; }
        public string? CalledZip { get; set; }
        public string? ApiVersion { get; set; }
        public string? CalledCity { get; set; }
        public string? CallStatus { get; set; }
        public string? From { get; set; }

        public string? AccountSid { get; set; }
        public string? CalledCountry { get; set; }
        public string? CallerCity { get; set; }
        public string? ToCity { get; set; }
        public string? FromCountry { get; set; }
        public string? Caller { get; set; }

        public string? FromCity { get; set; }
        public string? CalledState { get; set; }
        public string? FromZip { get; set; }
        public string? FromState { get; set; }
        public string? SipResponseCode { get; set; }
        public string? Duration { get; set; }

        public string? CallDuration { get; set; }
    }
}
