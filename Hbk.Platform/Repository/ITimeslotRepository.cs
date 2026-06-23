using Hbk.Database;
using Hbk.Models.DTO;

namespace Hbk.Platform.Repository
{
    public interface ITimeslotRepository
    {
        public Task<TimeslotDto> GetTimeslot(int timeslotId);
        public Task<List<TimeslotDto>> GetPracticeTimeslots();
        public Task Create(TimeslotDto timeslotDto);
        public Task Create(List<Timeslot> timeslots);
    }
}