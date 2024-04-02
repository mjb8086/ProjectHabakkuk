using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IAvailabilityRepository
    {
        public Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeek(int pracId, int weekNum);
        public Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeeks(int pracId, int[] weekNums);
        public Task<Dictionary<int, TimeslotAvailabilityDto>> GetPractitionerLookupForIndef(int pracId);
        public Task UpdateAvailabilityForIndef(int pracId, Dictionary<int, bool> tsAvaDict);
        public Task UpdatePracForWeek(int weekNum, int pracId, Dictionary<int, bool> tsAvaDict);
        public Task ClearPractitionerForWeek(int weekNum, int pracId);
        public Task ClearPractitionerForIndef(int pracId);
    }
}