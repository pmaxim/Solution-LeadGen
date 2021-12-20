using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface IVonageWebhookRepository : IBaseRepository<VonageWebhook>
    {
        IQueryable<VonageWebhook> VonageWebhooks { get; }
    }
}