namespace Domain.Models
{
    public static class Constants
    {
#if DEBUG
        public const string AppData = @"C:\Work2021\LeadGen\BlazorWeb\Server\App_Data";
#else
        public const string AppData = @"C:\Webs\leadgen.serverpipe.com\App_Data";
#endif
        public const string RoleAdmin = "admin";
        public const string RoleAgent = "agent";
        public const string RoleDeveloper = "developer";
        //Setting Error
        public const string ErrorEmail = "ErrorEmail";
        //vonage
        public const string BaseUrlRest = "https://rest.nexmo.com";
        public const string BaseUrlVoice = "https://api.nexmo.com/v1/calls";
        public const string BaseUrlVoicev2 = "https://api.nexmo.com/v2/calls";
        public const string AnswerVoice = "https://api.nexmo.com/v2/calls";

        public const int RingingTimer = 1;
        public const int RingingTwilioTimer = 1;
        public const string VonageAnswerUrl = "https://leadgen.serverpipe.com/sounds/answer.json";
        public const string VonageEventUrl = "https://leadgen.serverpipe.com/vonageVoice";
        public const string TwilioAnswerUrl = "https://leadgen.serverpipe.com/sounds/twilio.xml";
        public const string TwilioEventUrl = "https://leadgen.serverpipe.com/twilioStatus";
    }
}
