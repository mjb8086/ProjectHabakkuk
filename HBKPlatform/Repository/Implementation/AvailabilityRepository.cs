using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    public class AvailabilityRepository(ApplicationDbContext _db) : IAvailabilityRepository
    {
        public async Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeek(int pracId, int weekNum)
        {
            return await _db.TimeslotAvailabilities.Include("Timeslot")
                .Where(x => x.WeekNum == weekNum && x.PractitionerId == pracId)
                .Select(x => new TimeslotAvailabilityDto() {WeekNum = x.WeekNum, Availability = x.Availability, TimeslotId = x.TimeslotId}).ToListAsync();
        }
    
        public async Task<Dictionary<int, TimeslotAvailabilityDto>> GetAvailabilityLookupForIndef(int pracId)
        {
            return await _db.TimeslotAvailabilities.Include("Timeslot")
                .Where(x => x.IsIndefinite && x.PractitionerId == pracId)
                .ToDictionaryAsync(x => x.TimeslotId, x => new TimeslotAvailabilityDto() {Availability = x.Availability, IsIndefinite = x.IsIndefinite, TimeslotId = x.TimeslotId});
        }
    
        public async Task<List<TimeslotAvailabilityDto>> GetAvailabilityLookupForWeeks(int pracId, int[] weekNums)
        {
            return await _db.TimeslotAvailabilities.Include("Timeslot")
                .Where(x => weekNums.Contains(x.WeekNum) && x.PractitionerId == pracId)
                .Select(x => new TimeslotAvailabilityDto() { TimeslotId= x.TimeslotId, Availability = x.Availability, WeekNum = x.WeekNum}).ToListAsync();
        }

        public async Task UpdateAvailabilityForWeek(int weekNum, int pracId, Dictionary<int, bool> tsAvaDict)
        {
            // Ensure this week is current or in the future
            
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailabilities.Include("Timeslot").Where(x =>
                x.WeekNum == weekNum && x.PractitionerId == pracId).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecords(tsAvaDict, weekNum, pracId, false);
        
            await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }
    
        public async Task UpdateAvailabilityForIndef(int pracId, Dictionary<int, bool> tsAvaDict)
        {
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailabilities.Include("Timeslot").Where(x =>
                x.IsIndefinite  && x.PractitionerId == pracId).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecords(tsAvaDict, -1, pracId, true);
        
            if (newAva.Any()) await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Set availability on existing db records if any were changed in the tsAvaDict.
        /// NOTE: will remove elements from the tsAvaDict if there are any matches to the db.
        /// </summary>
        private void UpdateExistingRecords(Dictionary<int, bool> tsAvaDict, Dictionary<int, TimeslotAvailability> existingRecords)
        {
            foreach (var tsId in tsAvaDict.Keys)
            {
                if (existingRecords.ContainsKey(tsId))
                {
                    existingRecords[tsId].Availability = tsAvaDict[tsId] ? Enums.TimeslotAvailability.Available : Enums.TimeslotAvailability.Unavailable;
                    tsAvaDict.Remove(tsId);
                }
            }
        }

        private List<TimeslotAvailability> GetNewRecords(Dictionary<int, bool> tsAvaDict, int weekNum, int pracId, bool isIndefinite)
        {
            var newAva = new List<TimeslotAvailability>();
            foreach (var tsId in tsAvaDict.Keys)
            {
                newAva.Add(new TimeslotAvailability()
                {
                    TimeslotId = tsId, WeekNum = weekNum, PractitionerId = pracId, IsIndefinite = isIndefinite,
                    Availability = tsAvaDict[tsId]
                        ? Enums.TimeslotAvailability.Available
                        : Enums.TimeslotAvailability.Unavailable
                });
            }

            return newAva;
        }

        public async Task RevertAvailabilityForWeek(int weekNum, int pracId)
        {
            await _db.TimeslotAvailabilities.Include("Timeslot").Where(x => x.WeekNum == weekNum && x.PractitionerId == pracId).ExecuteDeleteAsync();
        }
    
        public async Task RevertAvailabilityForIndef(int pracId)
        {
            await _db.TimeslotAvailabilities.Include("Timeslot").Where(x => x.IsIndefinite && x.PractitionerId == pracId).ExecuteDeleteAsync();
        }

    }
}