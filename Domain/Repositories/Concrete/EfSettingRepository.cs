using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Concrete
{
    public class EfSettingRepository : EfBaseRepository<Setting>, ISettingRepository
    {
        public IQueryable<Setting> Settings => Context.Settings;

        public EfSettingRepository(EfDbContext db) : base(db)
        {
        }

        public async Task SettingAddSingle(Setting s)
        {
            var q = Settings.Where(z => z.Name == s.Name);
            var f = await q.AnyAsync();
            if (f)
            {
                var st = await q.FirstAsync();
                st.Value = s.Value;
            }
            else Context.Add(s);

            await SaveChangesAsync();
        }
    }
}
