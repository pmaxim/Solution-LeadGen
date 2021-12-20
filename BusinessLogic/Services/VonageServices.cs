using BlazorWeb.Shared.Lib;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vonage;
using Vonage.Applications;
using Vonage.Numbers;
using Vonage.Request;
using Vonage.Voice.Nccos.Endpoints;
using CallCommand = Vonage.Voice.CallCommand;
using CallResponse = Vonage.Voice.CallResponse;

namespace BusinessLogic.Services
{
    public interface IVonageServices
    {
        Task<NumbersSearchResponse> ApiGetPhoneNumbers(int accountId);
        Task<NumbersSearchResponse> GetPhoneNumbers(int accountId);
        Task<CallResponse> CallNumber(int accountId, string numberFrom, string numberTo);
        Task<Application> GetApplication(int accountId);
        Task<NumbersSearchResponse> GetApplicationNumber(int accountId);
        Task<ApplicationPage> GetApplications(int accountId);
    }

    public class VonageServices : IVonageServices
    {
        private readonly IAccountRepository _accountRepository;

        public VonageServices(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<NumbersSearchResponse> ApiGetPhoneNumbers(int accountId)
        {
            var account = await _accountRepository.GetAsync(accountId);
            var json = await WorkLib
                .GetHttp($"{Constants.BaseUrlRest}/account/numbers?api_key={account.Sid}&api_secret={account.Token}");
            return JsonConvert.DeserializeObject<NumbersSearchResponse>(json);
        }

        public async Task<NumbersSearchResponse> GetPhoneNumbers(int accountId)
        {
            //var client = await InitVonage(accountId);
            var account = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications).SingleAsync();

            var credentials = Credentials.FromApiKeyAndSecret(account.Sid, account.Token);
            var client = new VonageClient(credentials);

            var request = new NumberSearchRequest()
            {
                SearchPattern = (SearchPattern)0,
                Pattern = "1"
            };
           
            return await client.NumbersClient.GetOwnedNumbersAsync(request);
        }

        public async Task<Application> GetApplication(int accountId)
        {
            var account = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications).SingleAsync();

            var credentials = Credentials.FromApiKeyAndSecret(account.Sid, account.Token);
            var client = new VonageClient(credentials);

            var appId = account.AccountApplications.First().AppId;

            return await client.ApplicationClient.GetApplicationAsync(appId);
        }

        public async Task<ApplicationPage> GetApplications(int accountId)
        {
            var account = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications).SingleAsync();

            var credentials = Credentials.FromApiKeyAndSecret(account.Sid, account.Token);
            var client = new VonageClient(credentials);

            var appId = account.AccountApplications.First().AppId;

            var request = new ListApplicationsRequest()
            {
                PageSize = 100
            };

            return await client.ApplicationClient.ListApplicationsAsync(request);
        }

        //https://dashboard.nexmo.com/applications/d72d0bc4-8cdd-4051-944a-37cc8cde6201
        public async Task<NumbersSearchResponse> GetApplicationNumber(int accountId)
        {
            var account = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications).SingleAsync();

            var appId = account.AccountApplications.First().AppId;
            var path = $"{Constants.AppData}\\VonageKey\\{appId}\\private.key";
            var key = await System.IO.File.ReadAllTextAsync(path);

            var credentials = Credentials.FromAppIdAndPrivateKey(appId, key);
            var client = new VonageClient(credentials);

            var request = new NumberSearchRequest()
            {
                SearchPattern = (SearchPattern)0,
                Pattern = "1"
            };

            return await client.NumbersClient.GetOwnedNumbersAsync(request);
        }

        //https://developer.vonage.com/voice/voice-api/overview#getting-started
        public async Task<CallResponse> CallNumber(int accountId, string numberFrom, string numberTo)
        {
            var client = await InitVonage(accountId);

            var command = new CallCommand() { 
                To = new Endpoint[]
                {
                    new PhoneEndpoint { Number = numberTo }
                }, 
                From = new PhoneEndpoint
                {
                    Number = numberFrom
                }, 
                AnswerUrl = new[] { Constants.VonageAnswerUrl },
                EventUrl = new[] { Constants.VonageEventUrl },
                EventMethod = "POST",
                RingingTimer = Constants.RingingTimer
            };

            return await client.VoiceClient.CreateCallAsync(command);
        }

        private async Task<VonageClient> InitVonage(int accountId)
        {
            var account = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications).SingleAsync();

            var appId = account.AccountApplications.First().AppId;

            var path = $"{Constants.AppData}\\VonageKey\\{appId}\\private.key";
            var key = await System.IO.File.ReadAllTextAsync(path);

            var credentials = new Credentials(account.Sid, account.Token, appId, key);

            return new VonageClient(credentials);
        }
    }
}
