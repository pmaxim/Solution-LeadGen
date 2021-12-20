using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface ITwilioWebhookRepository : IBaseRepository<TwilioWebhook>
    {
        IQueryable<TwilioWebhook> TwilioWebhooks { get; }
    }
}