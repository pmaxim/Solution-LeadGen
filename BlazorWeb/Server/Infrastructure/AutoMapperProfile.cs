using AutoMapper;
using BlazorWeb.Server.Models;
using BlazorWeb.Shared.Models;
using Domain.Entities;

namespace BlazorWeb.Server.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<VonageVoiceEndPoint, VonageWebhook>();
            CreateMap<Account, AccountModel>();
            CreateMap<AccountModel, Account>();
            CreateMap<AccountApplication, AccountApplicationModel>();
            CreateMap<AccountApplicationModel, AccountApplication>();
            CreateMap<Account, AccountNumbersModel>();
            CreateMap<AccountNumbersModel, AccountNumbersItem>();

            CreateMap<NotificationSubscriptionViewModel, NotificationSubscription>();
            CreateMap<NotificationSubscription, NotificationSubscriptionViewModel>();

            CreateMap<Schedule, ScheduleViewModel>();
            CreateMap<ScheduleViewModel, Schedule>();

            CreateMap<CallIncoming, DashboardIncomingCallsItem>();
            CreateMap<DashboardIncomingCallsItem, CallIncoming>();

            CreateMap<TwilioWebhook, TwilioWebhookViewModel>();
        }
    }
}
