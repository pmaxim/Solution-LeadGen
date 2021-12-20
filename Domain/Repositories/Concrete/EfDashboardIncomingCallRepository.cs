using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfDashboardIncomingCallRepository : EfBaseRepository<DashboardIncomingCall>, IDashboardIncomingCallRepository
    {
        public IQueryable<DashboardIncomingCall> DashboardIncomingCalls => Context.DashboardIncomingCalls;

        public EfDashboardIncomingCallRepository(EfDbContext db) : base(db)
        {
        }
    }
}