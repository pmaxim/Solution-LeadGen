using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AutoMapper;
using BlazorWeb.Shared.Models;
using Domain.Repositories;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IAccountServices
    {
        Task<AccountsViewModel> GetAccounts(string userName, string role);
        Task<bool> Activate(int accountId);
        Task<List<AccountNumbersModel>> GetAccountNumbers(string userName);
        Task<AccountNumbersViewModel> GetAccountNumbersView(string userName);
        Task<AccountModel> GetAccount(int accountId);
        Task SaveAccount(AccountModel data);
        Task<bool> RemoveAccount(int accountId);
        Task<bool> ActivateApplication(int applicationId);
        Task<List<Account>> GetAllActiveAccounts();
    }

    public class AccountServices : IAccountServices
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountApplicationRepository _accountApplicationRepository;
        private readonly ITwilioServices _twilioServices;
        private readonly IVonageServices _vonageServices;
        private readonly IMapper _mapper;

        public AccountServices(IAccountRepository accountRepository,
            IAccountApplicationRepository accountApplicationRepository,
            ITwilioServices twilioServices,
            IVonageServices vonageServices,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _accountApplicationRepository = accountApplicationRepository;
            _twilioServices = twilioServices;
            _vonageServices = vonageServices;
            _mapper = mapper;
        }

        public async Task<AccountsViewModel> GetAccounts(string userName, string role)
        {
            var model = new AccountsViewModel();
            var list = await _accountRepository.GetAccountsAsNo(userName, role);

            foreach (var item in list.Select(p => _mapper.Map<AccountModel>(p)))
            {
                item.Provider = item.IsVonage ? ShareConstants.Vonage : ShareConstants.Twilio;
                model.List.Add(item);
            }

            return model;
        }

        public async Task<bool> Activate(int accountId)
        {
            return await _accountRepository.Activate(accountId);
        }

        public async Task<List<AccountNumbersModel>> GetAccountNumbers(string userName)
        {
            var m = new List<AccountNumbersModel>();
            var accounts = await _accountRepository.Accounts.AsNoTracking()
                .Where(z => z.IsActive && z.UserName == userName)
                .Include(z => z.AccountApplications)
                .ToListAsync();

            foreach (var account in accounts)
            {
                var item = _mapper.Map<AccountNumbersModel>(account);

                if (!item.IsVonage)
                {
                    m.Add(item);
                    continue;
                }

                foreach (var vonage in account.AccountApplications.Where(z => z.IsActive))
                {
                    var itemVonage = _mapper.Map<AccountNumbersModel>(account);
                    itemVonage.Sid = vonage.AppId;
                    m.Add(itemVonage);
                }
            }

            return await AddNumbers(m);
        }

        private async Task<List<AccountNumbersModel>> AddNumbers(List<AccountNumbersModel> m)
        {
            var model = new List<AccountNumbersModel>();

            foreach (var p in m)
            {
                if (p.IsVonage)
                {
                    var numbers = await _vonageServices?.GetPhoneNumbers(p.Id)!;
                    model.AddRange(numbers.Numbers.Select(number => new AccountNumbersModel
                    {
                        Id = p.Id,
                        IsVonage = p.IsVonage,
                        Sid = p.Sid,
                        Token = p.Token,
                        FromNexmo = number.Msisdn
                    }));
                }
                else
                {
                    var numbers = await _twilioServices?.GetPhoneNumbers(p.Id)!;
                    if (numbers != null)
                        model.AddRange(numbers.Select(number => new AccountNumbersModel
                        {
                            Id = p.Id,
                            IsVonage = p.IsVonage,
                            Sid = p.Sid,
                            Token = p.Token,
                            FromTwilio = number.PhoneNumber
                        }));
                }

                await Task.Delay(1000);
            }

            return model;
        }

        public async Task<AccountNumbersViewModel> GetAccountNumbersView(string userName)
        {
            var model = new AccountNumbersViewModel();
            var list = await GetAccountNumbers(userName);

            foreach (var p in list)
            {
                var item = _mapper.Map<AccountNumbersItem>(p);
                item.Phone = p.IsVonage ? p.FromNexmo : p.FromTwilio!.ToString();
                model.List.Add(item);
            }

            return model;
        }

        public async Task<AccountModel> GetAccount(int accountId)
        {
            var account = await _accountRepository.GetAccount(accountId);
            var model = _mapper.Map<AccountModel>(account);
            foreach (var application in account.AccountApplications)
            {
                model.List.Add(_mapper.Map<AccountApplicationModel>(application));
            }
            return model;
        }

        public async Task SaveAccount(AccountModel data)
        {
            switch (data.Id)
            {
                case > 0 when !data.IsVonage:
                {
                    var account = await _accountRepository.GetAsync(data.Id);
                    account.Sid = data.Sid;
                    account.Token = data.Token;
                    account.Title = data.Title;
                    //_accountRepository.Update(account);
                    await _accountRepository.SaveChangesAsync();
                    break;
                }
                case 0 when !data.IsVonage:
                {
                    var account = _mapper.Map<Account>(data);
                    _accountRepository.Create(account);
                    await _accountRepository.SaveChangesAsync();
                    break;
                }
                case 0 when data.IsVonage:
                {
                    var account = _mapper.Map<Account>(data);

                    foreach (var application in data.List)
                    {
                        account.AccountApplications.Add(_mapper.Map<AccountApplication>(application));
                    }

                    _accountRepository.Create(account);

                    await _accountRepository.SaveChangesAsync();
                    break;
                }
                case > 0 when data.IsVonage:
                {
                    var account = await _accountRepository.GetAsync(data.Id);
                    account.Sid = data.Sid;
                    account.Token = data.Token;
                    account.Title = data.Title;
                    _accountRepository.Update(account);

                    var currentApps = await _accountApplicationRepository.AccountApplications
                        .Where(z => z.Account.Id == data.Id).ToListAsync();

                    foreach (var application in data.List)
                    {
                        var app = currentApps.SingleOrDefault(z => z.Id == application.Id);
                        if (app == null) account.AccountApplications.Add(_mapper.Map<AccountApplication>(application));
                        else
                        {
                            app.IsActive = application.IsActive;
                            app.AppId = application.AppId;
                            app.Title = application.Title;
                        }
                    }
                    //Check removed
                    foreach (var app in currentApps.Where(app => data.List.All(z => z.Id != app.Id)))
                    {
                        _accountApplicationRepository.Remove(app);
                        var path = $"{Constants.AppData}\\VonageKey\\{app.AppId}";
                        Directory.Delete(path, true);
                    }
                    await _accountRepository.SaveChangesAsync();
                    break;
                }
            }
        }

        public async Task<bool> RemoveAccount(int accountId)
        {
            var account = await _accountRepository.GetAccount(accountId);

            foreach (var app in account.AccountApplications)
            {
                _accountApplicationRepository.Remove(app);
                var path = $"{Constants.AppData}\\VonageKey\\{app.AppId}";
                Directory.Delete(path, true);
            }

            _accountRepository.Remove(account);
            await _accountRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateApplication(int applicationId)
        {
            var application = await _accountApplicationRepository.GetAsync(applicationId);
            application.IsActive = !application.IsActive;
            await _accountApplicationRepository.SaveChangesAsync();
            return application.IsActive;
        }

        public async Task<List<Account>> GetAllActiveAccounts()
        {
            return await _accountRepository.Accounts
                .Where(z => z.IsActive && !z.IsVonage).ToListAsync();
        }
    }
}