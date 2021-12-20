using Domain.Entities;
using System.Linq;

namespace Domain.Repositories
{
    public interface IAccountApplicationRepository : IBaseRepository<AccountApplication>
    {
        IQueryable<AccountApplication> AccountApplications { get; }
    }
}