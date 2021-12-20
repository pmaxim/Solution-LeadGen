using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface ICallIncomingRepository : IBaseRepository<CallIncoming>
    {
        IQueryable<CallIncoming> CallIncomings { get; }
    }
}