using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface IDashboardIncomingCallRepository : IBaseRepository<DashboardIncomingCall>
    {
        IQueryable<DashboardIncomingCall> DashboardIncomingCalls { get; }
    }
}