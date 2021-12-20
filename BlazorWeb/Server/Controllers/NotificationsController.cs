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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;

        public NotificationsController(INotificationServices notificationServices)
        {
            _notificationServices = notificationServices;
        }

        [HttpPut("subscribe")]
        public async Task<NotificationSubscriptionViewModel> Subscribe(NotificationSubscriptionViewModel subscription)
        {
            // We're storing at most one subscription per user, so delete old ones.
            // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return subscription;

            //if one subscribe to user name
            //await _notificationServices.RemoveOldNotificationSubscription(userName);

            // Store new subscription
            subscription.UserId = userName;
            await _notificationServices.Create(subscription);

            return subscription;
        }

        [HttpPost("[action]")]
        public async Task<bool> SendNotification(NotificationSubjectViewModel subject)
        {
            var userName = User.Identity!.Name;
            if (string.IsNullOrEmpty(userName)) return false;
            await _notificationServices.SendNotificationAsync(subject, userName);
            return true;
        }
    }
}
