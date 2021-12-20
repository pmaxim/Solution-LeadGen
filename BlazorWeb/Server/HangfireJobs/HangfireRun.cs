using AutoMapper;
using BlazorWeb.Server.Hubs;
using BlazorWeb.Shared.Models;
using BusinessLogic.Services;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Twilio.Types;

namespace BlazorWeb.Server.HangfireJobs
{
    public interface IHangfireRun
    {
        Task Run(string userName);
    }

    public class HangfireRun : IHangfireRun
    {
        private readonly ILeadPhoneRepository _leadPhoneRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountServices _accountServices;
        private readonly ITwilioServices _twilioServices;
        private readonly IVonageServices _vonageServices;
        readonly IHubContext<TalkActive> _hubContext;
        private readonly IMapper _mapper;

        public HangfireRun(ILeadPhoneRepository leadPhoneRepository,
            ISettingRepository settingRepository,
            IAccountRepository accountRepository,
            ITwilioServices twilioServices,
            IVonageServices vonageServices,
            IAccountServices accountServices,
            IHubContext<TalkActive> hubContext,
            IMapper mapper)
        {
            _leadPhoneRepository = leadPhoneRepository;
            _settingRepository = settingRepository;
            _accountRepository = accountRepository;
            _twilioServices = twilioServices;
            _vonageServices = vonageServices;
            _accountServices = accountServices;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task Run(string userName)
        {
            var isStart = await CheckStart(userName);
            if (!isStart) return;

            var numbers = await _accountServices.GetAccountNumbers(userName);
            if (numbers.Count == 0)
            {
                await OffStart(userName);
                await _hubContext.Clients.All.SendAsync("HangfireRunRing", new HangfireRingModel { UserName = $"Please add Twilio/Vonage numbers" });
                return;
            }

            var phones = await _leadPhoneRepository.GetNoCall(userName);
            var index = 0;
            var current = 1;

            foreach (var phone in phones)
            {
                phone.IsCall = true;

                if (current % 10 == 0)
                {
                    await _leadPhoneRepository.SaveChangesAsync();
                    isStart = await CheckStart(userName);
                    if (!isStart) return;
                }

                var vp = numbers[index++];
                if (index > numbers.Count - 1) index = 0;

                try
                {
                    if (vp.IsVonage)
                    {
                        await _vonageServices?.CallNumber(vp.Id, vp.FromNexmo, phone.Phone)!;
                    }
                    else
                    {
                        var tw = (PhoneNumber)vp.FromTwilio!;
                        await _twilioServices?.CallNumber(vp.Id, tw, phone.Phone)!;
                    }
                }
                catch (Exception e)
                {
                    phone.Error = e.Message;
                    await _leadPhoneRepository.SaveChangesAsync();
                }

                await _hubContext.Clients.All.SendAsync("HangfireRunRing", new HangfireRingModel
                {
                    To = $"+1{phone.Phone.Trim()}",
                    UserName = phone.UserName,
                    IsVonage = vp.IsVonage,
                    From = vp.IsVonage ? $"+{vp.FromNexmo}" : vp.FromTwilio?.ToString(),
                    Total = phones.Count,
                    Current = current++
                });
                
                await Task.Delay(2000 / numbers.Count);
            }

            await OffStart(userName);
        }

        private async Task<bool> CheckStart(string userName)
        {
            return await _settingRepository.Settings
                .Where(z => z.Name == $"{ShareConstants.HangfireRunName}{userName}" &&
                            z.Value == ShareConstants.HangfireRunValue).AnyAsync();
        }

        private async Task OffStart(string userName)
        {
            await _settingRepository.SettingAddSingle(new Setting
            {
                Name = $"{ShareConstants.HangfireRunName}{userName}",
                Value = string.Empty
            });
        }
    }
}
