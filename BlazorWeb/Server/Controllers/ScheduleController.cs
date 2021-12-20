using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleServices _scheduleServices;

        public ScheduleController(IScheduleServices scheduleServices)
        {
            _scheduleServices = scheduleServices;
        }

        [HttpGet("[action]")]
        public async Task<ScheduleViewModel> GetSchedule()
        {
            var model = new ScheduleViewModel();
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return model;

            var role = string.Empty;
            if (User.IsInRole(Constants.RoleAdmin)) role = Constants.RoleAdmin;
            return await _scheduleServices.GetSchedule(userName, role);
        }

        [HttpGet("[action]")]
        public async Task ClearSchedule()
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return;
            await _scheduleServices.ClearSchedule(userName);
        }

        [HttpPost("[action]")]
        public async Task SaveSchedule(ScheduleViewModel data)
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return;
            await _scheduleServices.SaveSchedule(data, userName);
        }
    }
}
