using Domain.Entities;
using System.Linq;

namespace Domain.Repositories.Concrete
{
    public class EfAccountApplicationRepository : EfBaseRepository<AccountApplication>, IAccountApplicationRepository
    {
        public IQueryable<AccountApplication> AccountApplications => Context.AccountApplications;

        public EfAccountApplicationRepository(EfDbContext db) : base(db)
        {
        }
    }
}