using Hbk.Platform.Database;
using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Services
{
    public interface ITimeslotService
    {
        public Task CreateTimeslots(List<Timeslot> timeslots);
        public Task<SortedSet<TimeslotDto>> GetPopulatedFutureTimeslots();
    }
}