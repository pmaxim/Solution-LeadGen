using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories.Concrete
{
    public class EfNotificationSubscriptionRepository : EfBaseRepository<NotificationSubscription>, INotificationSubscriptionRepository
    {
        public IQueryable<NotificationSubscription> NotificationSubscriptions => Context.NotificationSubscriptions;

        public EfNotificationSubscriptionRepository(EfDbContext db) : base(db)
        {
        }

        public async Task<List<NotificationSubscription>> GetNotificationSubscriptions(string userId)
        {
            return await Context.NotificationSubscriptions
                .AsNoTracking()
                .Where(z => z.UserId == userId).ToListAsync();
        }
    }
}