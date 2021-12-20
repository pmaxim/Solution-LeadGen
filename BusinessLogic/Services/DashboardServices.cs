using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorWeb.Shared.Lib;
using BlazorWeb.Shared.Models;
using Domain.Entities;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IDashboardServices
    {
        Task<DashboardIncomingCallsViewModel> GetTableIncomingCalls(FilterDashboardModel filter, string userName, string role);
        Task RetrieveCallLogs(string userName, string role);
    }

    public class DashboardServices : IDashboardServices
    {
        private readonly IDashboardIncomingCallRepository _dashboardIncomingCallRepository;
        private readonly ICallIncomingRepository _callIncomingRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountServices _accountServices;
        private readonly ITwilioServices _twilioServices;
        private readonly IMapper _mapper;

        public DashboardServices(IDashboardIncomingCallRepository dashboardIncomingCallRepository,
            ICallIncomingRepository callIncomingRepository,
            IAccountRepository accountRepository,
            ITwilioServices twilioServices,
            IAccountServices accountServices,
            IMapper mapper)
        {
            _dashboardIncomingCallRepository = dashboardIncomingCallRepository;
            _callIncomingRepository = callIncomingRepository;
            _twilioServices = twilioServices;
            _accountServices = accountServices;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<DashboardIncomingCallsViewModel> GetTableIncomingCalls(FilterDashboardModel filter, string userName, string role)
        {
            var model = new DashboardIncomingCallsViewModel();
            var list = await _dashboardIncomingCallRepository.DashboardIncomingCalls
                .AsNoTracking()
                .Where(z => z.Date >= filter.FromDate &&
                            z.Date <= filter.ToDate)
                .Include(z=>z.CallIncomings)
                .Include(z=>z.Account)
                .ToListAsync();
            foreach (var dash in list)
            {
                if(role != Constants.RoleAdmin && dash.Account.UserName != userName) continue;
                foreach (var callIncoming in dash.CallIncomings)
                {
                    var item = _mapper.Map<DashboardIncomingCallsItem>(callIncoming);
                    item.Date = dash.Date.ToShortDateString();
                    item.Price = WorkLib.CorrectFloat(callIncoming.Price);
                    model.List.Add(item);
                }
            }

            model.List = model.List.OrderByDescending(z => z.Date).ToList();
            return model;
        }

        public async Task RetrieveCallLogs(string userName, string role)
        {
            List<Account> accounts;

            if(Constants.RoleAdmin == role) accounts = await _accountServices.GetAllActiveAccounts();
            else accounts = await _accountRepository.GetAccounts(userName, role);

            foreach (var account in accounts)
            {
                await _twilioServices.RetrieveCallLogs(account);
            }
        }
    }
}
