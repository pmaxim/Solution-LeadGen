using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories.Concrete
{
    public class EfScheduleRepository : EfBaseRepository<Schedule>, IScheduleRepository
    {
        public IQueryable<Schedule> Schedules => Context.Schedules;

        public EfScheduleRepository(EfDbContext db) : base(db)
        {
        }

        public async Task ScheduleAddSingle(Schedule s)
        {
            var q = Schedules.Where(z => z.UserName == s.UserName);
            var f = await q.AnyAsync();
            if (f)
            {
                var st = await q.FirstAsync();
                st.Start = s.Start;
                st.Finish = s.Finish;
                st.IsAveryDay = s.IsAveryDay;
            }
            else Context.Add(s);

            await SaveChangesAsync();
        }
    }
}