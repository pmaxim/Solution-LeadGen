using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nexmo.Api.Voice.Nccos
{
    [System.Obsolete("The Nexmo.Api.Voice.Nccos.NotifyAction class is obsolete. " +
           "References to it should be switched to the new Nexmo.Api.Voice.Nccos.NotifyAction class.")]
    public class NotifyAction : NccoAction
    {
        /// <summary>
        /// The JSON object body to send to your event URL
        /// </summary>
        [JsonProperty("payload")]
        public object Payload { get; set; }

        /// <summary>
        /// The URL to send events to. If you return an NCCO when you receive a notification, it will replace the current NCCO
        /// </summary>
        [JsonProperty("eventUrl")]
        public string[] EventUrl { get; set; }

        /// <summary>
        /// The HTTP method to use when sending payload to your eventUrl
        /// </summary>
        [JsonProperty("eventMethod")]
        public string EventMethod { get; set; }

        public NotifyAction()
        {
            Action = ActionType.notify;
        }
    }
}
