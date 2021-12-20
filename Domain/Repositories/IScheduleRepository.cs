using Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        IQueryable<Schedule> Schedules { get; }
        Task ScheduleAddSingle(Schedule s);
    }
}