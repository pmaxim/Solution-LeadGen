using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfVonageWebhookRepository : EfBaseRepository<VonageWebhook>, IVonageWebhookRepository
    {
        public IQueryable<VonageWebhook> VonageWebhooks => Context.VonageWebhooks;

        public EfVonageWebhookRepository(EfDbContext db) : base(db)
        {
        }
    }
}