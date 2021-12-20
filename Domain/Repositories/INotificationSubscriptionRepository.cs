using System.Collections.Generic;
using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface INotificationSubscriptionRepository : IBaseRepository<NotificationSubscription>
    {
        IQueryable<NotificationSubscription> NotificationSubscriptions { get; }
        Task<List<NotificationSubscription>> GetNotificationSubscriptions(string userId);
    }
}