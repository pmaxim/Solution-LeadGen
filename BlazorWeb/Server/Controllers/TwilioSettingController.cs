using AutoMapper;
using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles="admin,user")]
    [ApiController]
    [Route("[controller]")]
    public class TwilioSettingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITwilioServices _twilioServices;

        public TwilioSettingController(ITwilioServices twilioServices,
            IMapper mapper)
        {
            _twilioServices = twilioServices;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<AccountModel> GetAccountSid()
        {
            var m = new AccountModel
            {
                Flag = true,
                UserName = User.Identity!.Name
            };

            if (string.IsNullOrEmpty(m.UserName)) return m;
            var accounts = await _twilioServices.GetAccounts(m.UserName);
            if (accounts.Count == 0) return m;

            m.Sid = accounts.First().Sid;
            m.Flag = false;
            return m;
        }
    }
}
