using AutoMapper;
using BlazorWeb.Shared.Models;
using Domain.Entities;
using Domain.Repositories;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebPush;

namespace BusinessLogic.Services
{
    public interface INotificationServices
    {
        Task SendNotificationAsync(NotificationSubjectViewModel subject, string userName);
        Task Create(NotificationSubscriptionViewModel subscription);
    }

    public class NotificationServices : INotificationServices
    {
        private readonly INotificationSubscriptionRepository _notificationSubscriptionRepository;
        private readonly IMapper _mapper;

        public NotificationServices(INotificationSubscriptionRepository notificationSubscriptionRepository,
            IMapper mapper)
        {
            _notificationSubscriptionRepository = notificationSubscriptionRepository;
            _mapper = mapper;
        }

        public async Task Create(NotificationSubscriptionViewModel subscription)
        {
            var ns = _mapper.Map<NotificationSubscription>(subscription);
            _notificationSubscriptionRepository.Create(ns);
            await _notificationSubscriptionRepository.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(NotificationSubjectViewModel subject, string userName)
        {
            var subscriptions = await _notificationSubscriptionRepository.GetNotificationSubscriptions(userName);
            // For a real application, generate your own
            const string publicKey = "BByg5Mu5Zqii6F8mbISwyKjscdBBbGTwWtKWKf_jtEp8k9RWynN9ILXIbrCiTE90Abp4us9DGAnvCnvNfV8-siY";
            const string privateKey = "j8mMdd9pztmZk-PteNdw-FZGkplREvuXgkt-wFNpZ4Y";

            foreach (var subscription in subscriptions)
            {
                var pushSubscription = new PushSubscription(subscription.Url,
                    subscription.P256dh, subscription.Auth);
                var vapidDetails = new VapidDetails("mailto:<pmx@ukr.net>", publicKey, privateKey);
                var webPushClient = new WebPushClient();

                //var path = $"push-view/?subjectId={subject.Id}"; //client side view
                var path = $"server-view/?subjectId={subject.Id}"; //server view
                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        title = "LeadGen",
                        message = subject.Text,
                        url = path,
                        icon = "img/icon-512.png"
                    });
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync("Error sending push notification: " + ex.Message);
                }
            }
        }
    }
}
