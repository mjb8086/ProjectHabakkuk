using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// HBKPlatform Timeslot repository.
    /// ALL DEPRECATED
    /// 
    /// Author: Mark Brown
    /// Authored: 11/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class TimeslotRepository(ApplicationDbContext _db) : ITimeslotRepository
    {
        /*
        public async Task<TimeslotDto> GetTimeslot(int timeslotId)
        {
            return await _db.Timeslots.Where(x => x.Id == timeslotId).Select(x => new TimeslotDto()
            {
                TimeslotId = x.Id,
                Description = x.Description,
                DurationMinutes = x.Duration,
                Day = x.Day,
                Time = x.StartTime
            }).FirstOrDefaultAsync() ?? throw new IdxNotFoundException("Could not find timeslot Id");
        }

        public async Task<List<TimeslotDto>> GetPracticeTimeslots()
        {
            return await _db.Timeslots.OrderBy(x => x.Day).ThenBy(x => x.StartTime).Select(x => new TimeslotDto()
            {
                TimeslotId = x.Id,
                Description = x.Description,
                DurationMinutes = x.Duration,
                Day = x.Day,
                Time = x.StartTime
            }).ToListAsync();
        }

        public async Task Create(TimeslotDto timeslotDto)
        {
            // TODO: Check for clashes with other timeslots.
            await _db.AddAsync(DtoToDb(timeslotDto));
            await _db.SaveChangesAsync();
        }
    
        public async Task Create(List<Timeslot> timeslots)
        {
            // TODO: Check for clashes with other timeslots.
            await _db.AddRangeAsync(timeslots);
            await _db.SaveChangesAsync();
        }

        private Timeslot DtoToDb(TimeslotDto dto)
        {
            return new()
            {
                Day = dto.Day,
                Description = dto.Description,
                StartTime = dto.Time,
                Duration = dto.DurationMinutes
            };
        }
    */
    }
}
