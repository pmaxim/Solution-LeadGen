using System.Collections.Generic;
using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ILeadPhoneRepository : IBaseRepository<LeadPhone>
    {
        IQueryable<LeadPhone> LeadPhones { get; }
        Task<int> UploadPhones(List<string> phones, string userName);
        Task<int> UploadPhonesUnique(List<string> phones, string userName);
        Task<int> UploadPhonesSql(List<string> phones, string userName);
        Task<int> CountPhones(string userName, string role, bool isCall);
        Task RemovePhones(string userName, bool isCall);
        Task RemovePhonesSql(string userName, bool isCall);
        Task<List<LeadPhone>> GetNoCall(string userName);
    }
}