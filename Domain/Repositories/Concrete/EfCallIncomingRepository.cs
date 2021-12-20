using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfCallIncomingRepository : EfBaseRepository<CallIncoming>, ICallIncomingRepository
    {
        public IQueryable<CallIncoming> CallIncomings => Context.CallIncomings;

        public EfCallIncomingRepository(EfDbContext db) : base(db)
        {
        }
    }
}