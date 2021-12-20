using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface ISettingRepository : IBaseRepository<Setting>
    {
        IQueryable<Setting> Settings { get; }
        Task SettingAddSingle(Setting s);
    }
}
