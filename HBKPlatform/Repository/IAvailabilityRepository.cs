using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IAvailabilityRepository
{
    public Task<Dictionary<int, Enums.TimeslotAvailability>> GetAvailabilityLookupForWeek(int clinicId, int pracId, int weekNum);
    public Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeeks(int clinicId, int pracId, int[] weekNums);
    public Task UpdateAvailabilityForWeek(int weekNum, int pracId, int clinicId, Dictionary<int, bool> tsAvaDict);
    public Task RevertAvailabilityForWeek(int weekNum, int pracId, int clinicId);
}