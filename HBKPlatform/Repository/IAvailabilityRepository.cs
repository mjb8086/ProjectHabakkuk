using HBKPlatform.Globals;

namespace HBKPlatform.Repository;

public interface IAvailabilityRepository
{
    public Task<Dictionary<int, Enums.TimeslotAvailability>> GetAvailabilityLookupForWeek(int clinicId, int pracId, int weekNum);
    public Task UpdateAvailabilityForWeek(int weekNum, int pracId, int clinicId, Dictionary<int, bool> tsAvaDict);
}