using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

public class AvailabilityRepository(ApplicationDbContext _db) : IAvailabilityRepository
{
    public async Task<Dictionary<int, Enums.TimeslotAvailability>> GetAvailabilityLookupForWeek(int clinicId, int weekNum)
    {
        return await _db.TimeslotAvailabilities.Include("Timeslot")
            .Where(x => x.Timeslot.ClinicId == clinicId && x.WeekNum == weekNum)
            .ToDictionaryAsync(x => x.TimeslotId, x => x.Availability);
    }

}