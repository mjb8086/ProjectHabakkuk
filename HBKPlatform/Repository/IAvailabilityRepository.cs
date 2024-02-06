using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IAvailabilityRepository
{
    public Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeek(int clinicId, int pracId, int weekNum);
    public Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeeks(int clinicId, int pracId, int[] weekNums);
    public Task<Dictionary<int, TimeslotAvailabilityDto>> GetAvailabilityLookupForIndef(int clinicId, int pracId);
    public Task UpdateAvailabilityForIndef(int pracId, int clinicId, Dictionary<int, bool> tsAvaDict);
    public Task UpdateAvailabilityForWeek(int weekNum, int pracId, int clinicId, Dictionary<int, bool> tsAvaDict);
    public Task RevertAvailabilityForWeek(int weekNum, int pracId, int clinicId);
    public Task RevertAvailabilityForIndef(int pracId, int clinicId);
}