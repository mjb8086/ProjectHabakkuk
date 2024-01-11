using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// HBKPlatform Timeslot repository.
/// 
/// Author: Mark Brown
/// Authored: 11/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class TimeslotRepository(ApplicationDbContext _db) : ITimeslotRepository
{
    public async Task<TimeslotDto> GetTimeslot(int timeslotId)
    {
        return await _db.Timeslots.Where(x => x.Id == timeslotId).Select(x => new TimeslotDto()
        {
            Id = x.Id,
            ClinicId = x.ClinicId,
            Description = x.Description,
            Duration = x.Duration,
            Day = x.Day
        }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Could not find timeslot Id");
    }

    public async Task<List<TimeslotDto>> GetClinicTimeslots(int clinicId)
    {
        return await _db.Timeslots.Where(x => x.ClinicId == clinicId).OrderBy(x => x.Day).ThenBy(x => x.Time).Select(x => new TimeslotDto()
        {
            Id = x.Id,
            ClinicId = x.ClinicId,
            Description = x.Description,
            Duration = x.Duration,
            Day = x.Day
        }).ToListAsync();
    }

    public async Task Create(TimeslotDto timeslotDto)
    {
        // TODO: Check for clashes with other timeslots.
        var timeslot = new Timeslot();
        
        timeslot.ClinicId = timeslotDto.ClinicId;
        timeslot.Day = timeslotDto.Day;
        timeslot.Description = timeslotDto.Description;
        timeslot.Time = timeslotDto.Time;
        timeslot.Duration = timeslotDto.Duration;

        await _db.AddAsync(timeslot);
        await _db.SaveChangesAsync();
    }
}