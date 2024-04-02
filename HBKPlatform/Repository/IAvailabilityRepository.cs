using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IAvailabilityRepository
    {
        public Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeek(int pracId, int weekNum);
        public Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeeks(int pracId, int[] weekNums);
        public Task<Dictionary<int, TimeslotAvailabilityDto>> GetPractitionerLookupForIndef(int pracId);
        public Task UpdatePractitionerForIndef(int pracId, Dictionary<int, bool> tsAvaDict);
        public Task UpdatePracForWeek(int weekNum, int pracId, Dictionary<int, bool> tsAvaDict);
        public Task ClearPractitionerForWeek(int weekNum, int pracId);
        public Task ClearPractitionerForIndef(int pracId);
        
        
        public Task<List<TimeslotAvailabilityDto>> GetRoomLookupForWeek(int roomId, int weekNum);
        public Task<Dictionary<int, TimeslotAvailabilityDto>> GetRoomLookupForIndef(int roomId);
        public Task<List<TimeslotAvailabilityDto>> GetRoomLookupForWeeks(int roomId, int[] weekNums);
        public Task UpdateRoomForWeek(int weekNum, int roomId, Dictionary<int, bool> tsAvaDict);
        public Task UpdateRoomForIndef(int roomId, Dictionary<int, bool> tsAvaDict);
        public Task ClearRoomForWeek(int weekNum, int pracId);
        public Task ClearRoomForIndef(int pracId);
    }
}