using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfTwilioWebhookRepository : EfBaseRepository<TwilioWebhook>, ITwilioWebhookRepository
    {
        public IQueryable<TwilioWebhook> TwilioWebhooks => Context.TwilioWebhooks;

        public EfTwilioWebhookRepository(EfDbContext db) : base(db)
        {
        }
    }
}