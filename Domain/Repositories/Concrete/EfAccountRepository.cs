using System.Collections.Generic;
using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Concrete
{
    public class EfAccountRepository : EfBaseRepository<Account>, IAccountRepository
    {
        public IQueryable<Account> Accounts => Context.Accounts;

        public EfAccountRepository(EfDbContext db) : base(db)
        {
        }

        public async Task<List<Account>> GetAccounts(string userName, string role)
        {
            if (role == Constants.RoleAdmin)
                return await Context.Accounts
                    .OrderBy(z => z.UserName)
                    .ToListAsync();

            return await Context.Accounts
                .Where(z => z.UserName == userName)
                .ToListAsync();
        }

        public async Task<List<Account>> GetAccountsAsNo(string userName, string role)
        {
            if(role == Constants.RoleAdmin)
                return await Context.Accounts.AsNoTracking()
                    .OrderBy(z => z.UserName)
                    .ToListAsync();

            return await Context.Accounts.AsNoTracking()
                .Where(z => z.UserName == userName)
                .ToListAsync();
        }

        public async Task<Account> GetAccount(int accountId)
        {
            return await Context.Accounts.AsNoTracking()
                .Where(z => z.Id == accountId)
                .Include(z => z.AccountApplications)
                .SingleAsync();
        }

        public async Task<bool> Activate(int accountId)
        {
            var account = await Context.Accounts.Where(z => z.Id == accountId).SingleAsync();
            account.IsActive = !account.IsActive;
            await Context.SaveChangesAsync();
            return account.IsActive;
        }
    }
}