using Nexmo.Api.Voice;
using System.Threading.Tasks;

namespace BusinessLogic.Lib
{
    //https://developer.nexmo.com/api/voice/ncco
    public static class NexmoVoice
    {
        //private static readonly Logger Log = LogManager.GetCurrentClassLogger(); 
        public static async Task MakeWavCall(string to, string nexmoFromNumber, string nexmoName)
        {
            var results = Call.Do(new Call.CallCommand
            {
                to = new[] { new Call.Endpoint{
                        type = "phone",
                        number = to
                    }
                },
                from = new Call.Endpoint
                {
                    type = "phone",
                    number = nexmoFromNumber
                },
                answer_url = new[]
                {
                    nexmoName
                }
            });
        }
    }
}
