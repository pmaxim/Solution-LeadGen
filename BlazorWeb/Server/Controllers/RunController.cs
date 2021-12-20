using BlazorWeb.Server.HangfireJobs;
using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWeb.Server.Controllers
{
    [Authorize(Roles = "admin,agent,developer")]
    [ApiController]
    [Route("[controller]")]
    public class RunController : ControllerBase
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IScheduleServices _scheduleServices;

        public RunController(ISettingRepository settingRepository,
            IScheduleServices scheduleServices)
        {
            _settingRepository = settingRepository;
            _scheduleServices = scheduleServices;
        }

        [HttpGet("[action]")]
        public async Task<BoolString> RunRings()
        {
            var model = new BoolString();
            
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return model;

            var isShedule = await _scheduleServices.SeeShedule(userName);
            if (isShedule)
            {
                model.Value = "Scheduled launch";
                return model;
            }

            var isStart = await _scheduleServices.SeeRun(userName);
            if (isStart)
            {
                model.Value = "Calls already started";
                return model;
            }

            await _scheduleServices.AddRun(userName);

            model.Value = BackgroundJob.Enqueue<IHangfireRun>(z => z.Run(userName));
            model.Flag = true;
            return model;
        }

        [HttpGet("[action]")]
        public async Task StopRings()
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return;

            await _scheduleServices.StopRun(userName);
        }
    }
}
