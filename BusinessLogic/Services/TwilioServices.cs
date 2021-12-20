using System;
using Domain.Repositories;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BlazorWeb.Shared.Models;
using BusinessLogic.Lib;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BusinessLogic.Services
{
    public interface ITwilioServices
    {
        Task<IEnumerable<IncomingPhoneNumberResource>> GetPhoneNumbers(int accountId);
        Task<CallResource> CallNumber(int accountId, PhoneNumber numberFrom, string numberTo);
        Task RetrieveCallLogs(Account account);
    }

    public class TwilioServices : ITwilioServices
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDashboardIncomingCallRepository _dashboardIncomingCallRepository;
        private readonly ICallIncomingRepository _callIncomingRepository;

        public TwilioServices(IAccountRepository accountRepository,
            IDashboardIncomingCallRepository dashboardIncomingCallRepository,
            ICallIncomingRepository callIncomingRepository)
        {
            _accountRepository = accountRepository;
            _dashboardIncomingCallRepository = dashboardIncomingCallRepository;
            _callIncomingRepository = callIncomingRepository;
        }

        public async Task<IEnumerable<IncomingPhoneNumberResource>> GetPhoneNumbers(int accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);
            TwilioClient.Init(account.Sid, account.Token);
            try
            {
                var t = await IncomingPhoneNumberResource.ReadAsync();
                return t.ToList();
            }
            catch
            {
                return null;
            }
        }

        //https://www.twilio.com/docs/voice/quickstart/csharp#make-an-outgoing-phone-call-with-c
        //https://www.twilio.com/docs/voice/make-calls
        public async Task<CallResource> CallNumber(int accountId, PhoneNumber numberFrom, string numberTo)
        {
            var account = await _accountRepository.GetAsync(accountId);
            TwilioClient.Init(account.Sid, account.Token);
            var to = new PhoneNumber(numberTo);

            var statusCallbackEvent = new List<string>
            {
                "initiated",
                "ringing",
                "answered",
                "completed"
            };

            return await CallResource.CreateAsync(to, numberFrom,
                url: new Uri(Constants.TwilioAnswerUrl),
                statusCallbackMethod: Twilio.Http.HttpMethod.Get,
                statusCallback: new Uri(Constants.TwilioEventUrl),
                statusCallbackEvent: statusCallbackEvent,
                timeout: Constants.RingingTwilioTimer
            );
        }

        //https://www.twilio.com/docs/voice/tutorials/how-to-retrieve-call-logs-csharp
        public async Task RetrieveCallLogs(Account account)
        {
            var dash = new DashboardIncomingCall
            {
                Account = account,
                Date = DateTime.Now.AddMinutes(-2) //run 12:00 AM
            };

            TwilioClient.Init(dash.Account.Sid, dash.Account.Token);

            var numbers = await GetPhoneNumbers(account.Id);
            
            foreach (var number in numbers)
            {
                var calls = 
                    await CallResource.ReadAsync(
                        to:number.PhoneNumber,
                        startTimeAfter: dash.Date.StartOfDay(),
                        endTimeBefore: dash.Date.EndOfDay());
                
                var callIncoming = new CallIncoming
                {
                    To = number.PhoneNumber.ToString(),
                    Count = calls.Count()
                };

                foreach (var record in calls)
                {
                    callIncoming.Duration += Convert.ToInt32(record.Duration);
                    
                    if (float.TryParse(record.Price, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out var result))
                    {
                        callIncoming.Price += result;
                    };
                }
                dash.CallIncomings.Add(callIncoming);
            }

            var q =  _dashboardIncomingCallRepository.DashboardIncomingCalls
                .Where(z => z.Account.Id == dash.Account.Id &&
                            z.Date.Date == dash.Date.Date);
            var f = await q.AnyAsync();

            if (f)
            {
                var db = await q.Include(z=>z.CallIncomings).FirstAsync();
                foreach (var callIncoming in db.CallIncomings)
                {
                    _callIncomingRepository.Remove(callIncoming);
                }
                _dashboardIncomingCallRepository.Remove(db);
            }

            _dashboardIncomingCallRepository.Create(dash);
            await _dashboardIncomingCallRepository.SaveChangesAsync();
        }
    }
}
