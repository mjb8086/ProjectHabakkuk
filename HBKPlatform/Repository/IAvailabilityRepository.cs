using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IAvailabilityRepository
    {
        public Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeek(int pracId, int weekNum);
        public Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeeks(int pracId, int[] weekNums);
        public Task<Dictionary<int, TimeslotAvailabilityDto>> GetAvailabilityLookupForIndef(int pracId);
        public Task UpdateAvailabilityForIndef(int pracId, Dictionary<int, bool> tsAvaDict);
        public Task UpdateAvailabilityForWeek(int weekNum, int pracId, Dictionary<int, bool> tsAvaDict);
        public Task RevertAvailabilityForWeek(int weekNum, int pracId);
        public Task RevertAvailabilityForIndef(int pracId);
    }
}