using HBKPlatform.Globals;

namespace HBKPlatform.Repository;

public interface IAvailabilityRepository
{
    public Task<Dictionary<int, Enums.TimeslotAvailability>> GetAvailabilityLookupForWeek(int clinicId, int weekNum);
}