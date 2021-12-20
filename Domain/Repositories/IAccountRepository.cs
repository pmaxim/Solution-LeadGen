using System.Collections.Generic;
using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        IQueryable<Account> Accounts { get; }
        Task<List<Account>> GetAccounts(string userName, string role);
        Task<List<Account>> GetAccountsAsNo(string userName, string role);
        Task<Account> GetAccount(int accountId);
        Task<bool> Activate(int accountId);
    }
}