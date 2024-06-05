using HBKPlatform.Database;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services
{
    public interface ITimeslotService
    {
        public Task<SortedSet<TimeslotDto>> GetPopulatedFutureTimeslots();
    }
}