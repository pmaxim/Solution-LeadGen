using System.Net.NetworkInformation;
using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Repositories;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace BlazorWeb.Server.HangfireJobs
{
    public interface IHangfireScheduleRun
    {
        Task Run();
    }

    public class HangfireScheduleRun : IHangfireScheduleRun
    {
        private readonly IScheduleServices _scheduleServices;
        private readonly IScheduleRepository _scheduleRepository;

        public HangfireScheduleRun(
            IScheduleRepository scheduleRepository,
            IScheduleServices scheduleServices)
        {
            _scheduleServices = scheduleServices;
            _scheduleRepository = scheduleRepository;
        }

        //http://odinserj.net/2014/05/21/hangfire-0.8.2-released/
        //[DisableConcurrentExecution(10 * 60)]
        public async Task Run()
        {
            await PingWithHttpClient();

            var schedules = await _scheduleRepository.Schedules.AsNoTracking()
                .ToListAsync();
            foreach (var schedule in schedules)
            {
                var isRun = await _scheduleServices.SeeRun(schedule.UserName);
                if (isRun && !schedule.IsAveryDay && schedule.Finish <= DateTime.Now)
                {
                    await _scheduleServices.StopRun(schedule.UserName);
                } else 
                if (!isRun && !schedule.IsAveryDay && schedule.Start <= DateTime.Now && 
                           schedule.Finish >= DateTime.Now)
                {
                    await _scheduleServices.AddRun(schedule.UserName);
                    BackgroundJob.Enqueue<IHangfireRun>(z => z.Run(schedule.UserName));
                }else 
                if (isRun && schedule.IsAveryDay && schedule.Finish.TimeOfDay <= DateTime.Now.TimeOfDay)
                {
                    await _scheduleServices.StopRun(schedule.UserName);
                }
                else
                if (!isRun && schedule.IsAveryDay && schedule.Start.TimeOfDay <= DateTime.Now.TimeOfDay 
                    && schedule.Finish.TimeOfDay >= DateTime.Now.TimeOfDay)
                {
                    await _scheduleServices.AddRun(schedule.UserName);
                    BackgroundJob.Enqueue<IHangfireRun>(z => z.Run(schedule.UserName));
                }
            }
        }

        //https://www.code4it.dev/blog/ping-endpoint-csharp
        private static async Task<bool> PingAsync()
        {
            var ping = new Ping();
            var result = await ping.SendPingAsync("leadgen.serverpipe.com");
            return result.Status == IPStatus.Success;
        }

        public static async Task<bool> PingWithHttpClient()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(ShareConstants.Domain),
                Method = HttpMethod.Head
            };
            var result = await httpClient.SendAsync(request);
            return result.IsSuccessStatusCode;
        }
    }
}
