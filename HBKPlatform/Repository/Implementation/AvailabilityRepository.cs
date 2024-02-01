using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

public class AvailabilityRepository(ApplicationDbContext _db) : IAvailabilityRepository
{
    public async Task<Dictionary<int, Enums.TimeslotAvailability>> GetAvailabilityLookupForWeek(int clinicId, int pracId, int weekNum)
    {
        return await _db.TimeslotAvailabilities.Include("Timeslot")
            .Where(x => x.Timeslot.ClinicId == clinicId && x.WeekNum == weekNum && x.PractitionerId == pracId)
            .ToDictionaryAsync(x => x.TimeslotId, x => x.Availability);
    }

    public async Task UpdateAvailabilityForWeek(int weekNum, int pracId, int clinicId, Dictionary<int, bool> tsAvaDict)
    {
        // 1. Find and update any existing availability records
        var existingRecords = await _db.TimeslotAvailabilities.Include("Timeslot").Where(x =>
                x.WeekNum == weekNum && x.Timeslot.ClinicId == clinicId && x.PractitionerId == pracId)
            .ToDictionaryAsync(x => x.TimeslotId, x => x.Availability);

        foreach (var tsId in existingRecords.Keys)
        {
            if (tsAvaDict.TryGetValue(tsId, out bool isAvailable))
            {
                existingRecords[tsId] = isAvailable
                    ? Enums.TimeslotAvailability.Available
                    : Enums.TimeslotAvailability.Unavailable;
                tsAvaDict.Remove(tsId);
            }
        }

        // 2. Create any new records 
        var newAva = new List<TimeslotAvailability>();
        foreach (var tsId in tsAvaDict.Keys)
        {
            newAva.Add(new TimeslotAvailability()
            {
                TimeslotId = tsId, WeekNum = weekNum, PractitionerId = pracId,
                Availability = tsAvaDict[tsId]
                    ? Enums.TimeslotAvailability.Available
                    : Enums.TimeslotAvailability.Unavailable
            });
        }
        await _db.AddRangeAsync(newAva);
        await _db.SaveChangesAsync();
    }

}