using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlazorWeb.Shared.Models;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services
{
    public interface IScheduleServices
    {
        Task<ScheduleViewModel> GetSchedule(string userName, string role);
        Task SaveSchedule(ScheduleViewModel data, string userName);
        Task ClearSchedule(string userName);
        Task<bool> SeeRun(string userName);
        Task AddRun(string userName);
        Task StopRun(string userName);
        Task<bool> SeeShedule(string userName);
    }

    public class ScheduleServices : IScheduleServices
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;

        public ScheduleServices(IScheduleRepository scheduleRepository,
            ISettingRepository settingRepository,
            IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _settingRepository = settingRepository;
            _mapper = mapper;
        }

        public async Task AddRun(string userName)
        {
            await _settingRepository.SettingAddSingle(new Setting
            {
                Name = $"{ShareConstants.HangfireRunName}{userName}",
                Value = ShareConstants.HangfireRunValue
            });
        }

        public async Task ClearSchedule(string userName)
        {
            var schedule = await _scheduleRepository.Schedules.AsNoTracking()
                .Where(z => z.UserName == userName).SingleOrDefaultAsync();
            if (schedule == null) return;
            _scheduleRepository.Remove(schedule);
            await _scheduleRepository.SaveChangesAsync();
        }

        public async Task<ScheduleViewModel> GetSchedule(string userName, string role)
        {
            var schedule = await _scheduleRepository.Schedules.AsNoTracking()
                .Where(z => z.UserName == userName).SingleOrDefaultAsync();
            if (schedule == null) return new ScheduleViewModel();

            var model = _mapper.Map<ScheduleViewModel>(schedule);
            return model;
        }

        public async Task SaveSchedule(ScheduleViewModel data, string userName)
        {
            var model = _mapper.Map<Schedule>(data);
            model.UserName = userName;
            await _scheduleRepository.ScheduleAddSingle(model);
        }

        public async Task<bool> SeeRun(string userName)
        {
            return await _settingRepository.Settings
                .Where(z => z.Name == $"{ShareConstants.HangfireRunName}{userName}" &&
                            z.Value == ShareConstants.HangfireRunValue).AnyAsync();
        }

        public async Task<bool> SeeShedule(string userName)
        {
            var schedule = await _scheduleRepository.Schedules.AsNoTracking()
                .Where(z => z.UserName == userName).SingleOrDefaultAsync();
            if (schedule == null) return false;
            return !(schedule.Start < DateTime.Now && 
                   schedule.Finish < DateTime.Now);
        }

        public async Task StopRun(string userName)
        {
            await _settingRepository.SettingAddSingle(new Setting
            {
                Name = $"{ShareConstants.HangfireRunName}{userName}",
                Value = string.Empty
            });
        }
    }
}
