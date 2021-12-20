namespace BlazorWeb.Shared.Models
{
    public class TwilioNumbersViewModel
    {
        public int Count { get; set; }
        public List<TwilioNumbersViewItem> List { get; set; } = new List<TwilioNumbersViewItem>();
    }

    public class TwilioNumbersViewItem
    {
        public string? FriendlyName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Sid { get; set; }
        public string? SmsUrl { get; set; }
        public string? VoiceUrl { get; set; }
        public string? StatusCallback { get; set; }
        public Capabilities? Capabilities { get; set; }
        public bool IsMms { get; set; }
        public bool IsSms { get; set; }
        public bool IsVoice { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public string? RowClass => !IsActive ? "table-danger" : null;
        public string? TextCapabilities => BuildCapabilities();
        public string? UseFor => BuildUseFor();

        private string BuildCapabilities()
        {
            var s = string.Empty;
            if (Capabilities!.Mms) s = "MMS ";
            if (Capabilities.Sms) s += "SMS ";
            if (Capabilities.Voice) s += "VOICE ";
            return s;
        }
        private string BuildUseFor()
        {
            var s = string.Empty;
            if (IsMms) s = "MMS ";
            if (IsSms) s += "SMS ";
            if (IsVoice) s += "VOICE ";
            return s;
        }
    }

    public class Capabilities
    {
        public bool Mms { get; set; }
        public bool Sms { get; set; }
        public bool Voice { get; set; }
    }
}
