using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = $"{Constants.RoleAdmin},{Constants.RoleAgent}")]
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AccountsController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpGet("[action]")]
        public async Task<AccountsViewModel> GetAccounts()
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return new AccountsViewModel();

            var role = string.Empty;
            if(User.IsInRole(Constants.RoleAdmin)) role = Constants.RoleAdmin;
            return await _accountServices.GetAccounts(userName, role);
        }

        [HttpGet("[action]")]
        public async Task<bool> Activate(int accountId)
        {
            return await _accountServices.Activate(accountId);
        }

        [HttpGet("[action]")]
        public async Task<AccountNumbersViewModel> GetAccountNumbers()
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return new AccountNumbersViewModel();

            return await _accountServices.GetAccountNumbersView(userName);
        }

        [HttpGet("[action]")]
        public async Task<AccountModel> GetAccount(int accountId)
        {
            return await _accountServices.GetAccount(accountId);
        }

        [HttpPost("[action]")]
        public async Task<bool> SaveAccount(AccountModel data)
        {
            if (data.Id == 0)
            {
                data.UserName = User.Identity!.Name;
                if (string.IsNullOrEmpty(data.UserName)) return false;
            }
            await _accountServices.SaveAccount(data);
            return true;
        }

        [HttpGet("[action]")]
        public async Task<bool> RemoveAccount(int accountId)
        {
            return await _accountServices.RemoveAccount(accountId);
        }
    }
}
