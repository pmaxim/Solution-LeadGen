using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories.Concrete
{
    public class EfLeadPhoneRepository : EfBaseRepository<LeadPhone>, ILeadPhoneRepository
    {
        public IQueryable<LeadPhone> LeadPhones => Context.LeadPhones;

        public EfLeadPhoneRepository(EfDbContext db) : base(db)
        {
        }

        public async Task<int> UploadPhones(List<string> phones, string userName)
        {
            var list = phones.Select(phone => new LeadPhone { Phone = phone, UserName = userName}).DistinctBy(z=>z.Phone).ToList();
            await Context.AddRangeAsync(list);
            await Context.SaveChangesAsync();
            return list.Count;
        }

        public async Task<int> UploadPhonesSql(List<string> phones, string userName)
        {
            var list = phones.Select(phone => new LeadPhone { Phone = phone, UserName = userName }).DistinctBy(z => z.Phone).ToList();

            foreach (var phone in phones)
            {
                await Context.Database
                    .ExecuteSqlInterpolatedAsync($"INSERT INTO LeadPhones (Phone,IsCall,UserName) VALUES ({phone},0,{userName})");
            }

            return list.Count;
        }

        public async Task<int> UploadPhonesUnique(List<string> phones, string userName)
        {
            var list = phones.Select(phone => new LeadPhone { Phone = phone, UserName = userName}).DistinctBy(z=>z.Phone).ToList();
            var missingPhones = 
                list.Where(z => !Context.LeadPhones.Any(p => p.Phone == z.Phone && z.UserName == userName)).ToList();
            if (missingPhones.Count == 0) return 0;
            await Context.AddRangeAsync(missingPhones);
            await Context.SaveChangesAsync();
            return missingPhones.Count;
        }

        public async Task<int> CountPhones(string userName, string role, bool isCall)
        {
            if (role == Constants.RoleAdmin)
            {
                return await Context.LeadPhones.Where(z => z.IsCall == isCall).CountAsync();
            }
            else
            {
                return await Context.LeadPhones.Where(z => z.IsCall == isCall && z.UserName == userName).CountAsync();
            }
        }

        public async Task RemovePhones(string userName, bool isCall)
        {
            Context.LeadPhones.RemoveRange(Context.LeadPhones.Where(z => z.IsCall == isCall && z.UserName == userName));

            await Context.SaveChangesAsync();
        }

        //https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
        //https://metanit.com/sharp/entityframeworkcore/6.1.php
        public async Task RemovePhonesSql(string userName, bool isCall)
        {
            await Context.Database
                .ExecuteSqlInterpolatedAsync($"DELETE FROM LeadPhones WHERE IsCall={isCall} AND UserName={userName}");
        }

        public async Task<List<LeadPhone>> GetNoCall(string userName)
        {
            return await Context.LeadPhones
                .Where(z => !z.IsCall && z.UserName == userName)
                .ToListAsync();
        }
    }
}