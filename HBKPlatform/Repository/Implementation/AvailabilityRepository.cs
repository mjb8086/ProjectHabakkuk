using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    public class AvailabilityRepository(ApplicationDbContext _db) : IAvailabilityRepository
    {
        //////////////////////////////////////////////////////////////////////////////// 
        // BEGIN PRACTITIONER AVAILABILITY
        ////////////////////////////////////////////////////////////////////////////////
         
        public async Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeek(int pracId, int weekNum)
        {
            return await _db.TimeslotAvailability.Include("Timeslot")
                .Where(x => x.WeekNum == weekNum && x.PractitionerId == pracId && x.Entity == Enums.AvailabilityEntity.Practitioner)
                .Select(x => new TimeslotAvailabilityDto() {WeekNum = x.WeekNum, Availability = x.Availability, TimeslotId = x.TimeslotId}).ToListAsync();
        }
    
        public async Task<Dictionary<int, TimeslotAvailabilityDto>> GetPractitionerLookupForIndef(int pracId)
        {
            return await _db.TimeslotAvailability.Include("Timeslot")
                .Where(x => x.IsIndefinite && x.PractitionerId == pracId && x.Entity == Enums.AvailabilityEntity.Practitioner)
                .ToDictionaryAsync(x => x.TimeslotId, x => new TimeslotAvailabilityDto() {Availability = x.Availability, IsIndefinite = x.IsIndefinite, TimeslotId = x.TimeslotId});
        }
    
        public async Task<List<TimeslotAvailabilityDto>> GetPractitionerLookupForWeeks(int pracId, int[] weekNums)
        {
            return await _db.TimeslotAvailability.Include("Timeslot")
                .Where(x => weekNums.Contains(x.WeekNum) && x.PractitionerId == pracId && x.Entity == Enums.AvailabilityEntity.Practitioner)
                .Select(x => new TimeslotAvailabilityDto() { TimeslotId= x.TimeslotId, Availability = x.Availability, WeekNum = x.WeekNum}).ToListAsync();
        }

        public async Task UpdatePracForWeek(int weekNum, int pracId, Dictionary<int, bool> tsAvaDict)
        {
            // Ensure this week is current or in the future
            
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailability.Include("Timeslot").Where(x =>
                x.WeekNum == weekNum && x.PractitionerId == pracId && x.Entity == Enums.AvailabilityEntity.Practitioner).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecordsPractitioner(tsAvaDict, weekNum, pracId, false);
        
            await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }
    
        public async Task UpdatePractitionerForIndef(int pracId, Dictionary<int, bool> tsAvaDict)
        {
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailability.Include("Timeslot").Where(x =>
                x.IsIndefinite  && x.PractitionerId == pracId && x.Entity == Enums.AvailabilityEntity.Practitioner).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecordsPractitioner(tsAvaDict, -1, pracId, true);
        
            if (newAva.Any()) await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }
        
        public async Task ClearPractitionerForWeek(int weekNum, int pracId)
        {
            await _db.TimeslotAvailability.Include("Timeslot").Where(x => x.WeekNum == weekNum && x.PractitionerId == pracId).ExecuteDeleteAsync();
        }
    
        public async Task ClearPractitionerForIndef(int pracId)
        {
            await _db.TimeslotAvailability.Include("Timeslot").Where(x => x.IsIndefinite && x.PractitionerId == pracId).ExecuteDeleteAsync();
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // BEGIN ROOM AVAILABILITY
        //////////////////////////////////////////////////////////////////////////////// 
        
        public async Task<List<TimeslotAvailabilityDto>> GetRoomLookupForWeek(int roomId, int weekNum)
        {
            return await _db.TimeslotAvailability.Include("Timeslot")
                .Where(x => x.WeekNum == weekNum && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room)
                .Select(x => new TimeslotAvailabilityDto() {WeekNum = x.WeekNum, Availability = x.Availability, TimeslotId = x.TimeslotId}).ToListAsync();
        }
    
        public async Task<Dictionary<int, TimeslotAvailabilityDto>> GetRoomLookupForIndef(int roomId)
        {
            return await _db.TimeslotAvailability.Include("Timeslot")
                .Where(x => x.IsIndefinite && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room)
                .ToDictionaryAsync(x => x.TimeslotId, x => new TimeslotAvailabilityDto() {Availability = x.Availability, IsIndefinite = x.IsIndefinite, TimeslotId = x.TimeslotId});
        }
    

        /// <summary>
        /// service should ensure this week is current or in the future
        /// </summary>
        public async Task UpdateRoomForWeek(int weekNum, int roomId, Dictionary<int, bool> tsAvaDict)
        {
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailability.Include("Timeslot").Where(x =>
                x.WeekNum == weekNum && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecordsRoom(tsAvaDict, weekNum, roomId, false);
        
            await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }
    
        public async Task UpdateRoomForIndef(int roomId, Dictionary<int, bool> tsAvaDict)
        {
            // 1. Find and update any existing availability records
            var existingRecords = await _db.TimeslotAvailability.Include("Timeslot").Where(x =>
                x.IsIndefinite  && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room).ToDictionaryAsync(x => x.TimeslotId);

            UpdateExistingRecords(tsAvaDict, existingRecords);
        
            // 2. Create any new records 
            var newAva = GetNewRecordsRoom(tsAvaDict, -1, roomId, true);
        
            if (newAva.Any()) await _db.AddRangeAsync(newAva);
            await _db.SaveChangesAsync();
        }
        
        public async Task ClearRoomForWeek(int weekNum, int pracId)
        {
            await _db.TimeslotAvailability.Include("Timeslot").Where(x => x.WeekNum == weekNum && x.RoomId == pracId).ExecuteDeleteAsync();
        }
    
        public async Task ClearRoomForIndef(int pracId)
        {
            await _db.TimeslotAvailability.Include("Timeslot").Where(x => x.IsIndefinite && x.RoomId == pracId).ExecuteDeleteAsync();
        }

        /// <summary>
        /// For a room to be 'unavailable', it must either have an explicit Timeslot Availability of Unavailable, or
        /// there is no availability record in the Db.
        /// This method checks the DB for any 'available' availability on the timeslot, if there is, it will return false.
        /// </summary>
        public async Task<bool> IsRoomUnavailableForWeekAnyTenancy(int roomId, int weekNum, int timeslotId)
        {
            // Any per-week availability?
            var isPerWeekAvailable = await _db.TimeslotAvailability.IgnoreQueryFilters().Where(x =>
                x.TimeslotId == timeslotId && x.WeekNum == weekNum && x.RoomId == roomId &&
                x.Entity == Enums.AvailabilityEntity.Room && x.Availability == Enums.TimeslotAvailability.Available).AnyAsync();
            
            // If not, check Indef. If any Indef exists and is available, return false. Else true.
             var isIndefAvailable = await _db.TimeslotAvailability.IgnoreQueryFilters().Where(x =>
                x.TimeslotId == timeslotId && x.RoomId == roomId && x.IsIndefinite &&
                x.Entity == Enums.AvailabilityEntity.Room && x.Availability == Enums.TimeslotAvailability.Available).AnyAsync();
             
             return !(isPerWeekAvailable || isIndefAvailable);
        }
        
        public async Task<List<TimeslotAvailabilityDto>> GetRoomLookupForWeeksAnyTenancy(int roomId, int[] weekNums)
        {
            return await _db.TimeslotAvailability.Include("Timeslot").IgnoreQueryFilters()
                .Where(x => weekNums.Contains(x.WeekNum) && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room)
                .Select(x => new TimeslotAvailabilityDto() { TimeslotId= x.TimeslotId, Availability = x.Availability, WeekNum = x.WeekNum}).ToListAsync();
        }
        
        public async Task<Dictionary<int, TimeslotAvailabilityDto>> GetRoomLookupForIndefAnyTenancy(int roomId)
        {
            return await _db.TimeslotAvailability.Include("Timeslot").IgnoreQueryFilters()
                .Where(x => x.IsIndefinite && x.RoomId == roomId && x.Entity == Enums.AvailabilityEntity.Room)
                .ToDictionaryAsync(x => x.TimeslotId, x => new TimeslotAvailabilityDto() {Availability = x.Availability, IsIndefinite = x.IsIndefinite, TimeslotId = x.TimeslotId});
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // HELPERS
        ////////////////////////////////////////////////////////////////////////////////
        
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

        private List<TimeslotAvailability> GetNewRecordsPractitioner(Dictionary<int, bool> tsAvaDict, int weekNum, int pracId, bool isIndefinite)
        {
            var newAva = new List<TimeslotAvailability>();
            foreach (var tsId in tsAvaDict.Keys)
            {
                newAva.Add(new TimeslotAvailability()
                {
                    TimeslotId = tsId, WeekNum = weekNum, PractitionerId = pracId, IsIndefinite = isIndefinite,
                    Entity = Enums.AvailabilityEntity.Practitioner,
                    Availability = tsAvaDict[tsId]
                        ? Enums.TimeslotAvailability.Available
                        : Enums.TimeslotAvailability.Unavailable
                });
            }
            return newAva;
        }
        
        private List<TimeslotAvailability> GetNewRecordsRoom(Dictionary<int, bool> tsAvaDict, int weekNum, int roomId, bool isIndefinite)
        {
            var newAva = new List<TimeslotAvailability>();
            foreach (var tsId in tsAvaDict.Keys)
            {
                newAva.Add(new TimeslotAvailability()
                {
                    TimeslotId = tsId, WeekNum = weekNum, RoomId = roomId, IsIndefinite = isIndefinite, 
                    Entity = Enums.AvailabilityEntity.Room,
                    Availability = tsAvaDict[tsId]
                        ? Enums.TimeslotAvailability.Available
                        : Enums.TimeslotAvailability.Unavailable
                });
            }
            return newAva;
        }
    }
}