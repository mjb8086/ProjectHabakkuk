using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

public class RoomReservationRepository (ApplicationDbContext _db): IRoomReservationRepository
{
    public async Task Create(RoomReservationDto reservation)
    {
        await _db.RoomReservations.AddAsync(new RoomReservation() 
            {
                RoomId = reservation.RoomId,
                ClinicId = reservation.ClinicId,
                TimeslotId = reservation.TimeslotId,
                PractitionerId = reservation.PractitionerId,
                PracticeNote = reservation.PracticeNote,
                WeekNum = reservation.WeekNum,
                ReservationStatus = Enums.ReservationStatus.Requested
            });
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Update reservation - approve, deny or cancel. For clinic use, so will not consider TenancyId because the
    /// reservation was originally created by the practitioner.
    /// </summary>
    public async Task UpdateStatusClinic(int reservationId, Enums.ReservationStatus status, string? clinicNote)
    {
        await _db.RoomReservations.IgnoreQueryFilters().Where(x => x.Id == reservationId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.ReservationStatus, status).SetProperty(p => p.ClinicNote, clinicNote));
    }
    
    public async Task UpdateStatusPractitioner(int reservationId, Enums.ReservationStatus status)
    {
        await _db.RoomReservations.Where(x => x.Id == reservationId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.ReservationStatus, status));
    }

    /// <summary>
    /// Get upcoming reservations for the current clinic. Ignore query filters because these will have been created
    /// by another tenant - namely, the practitioner.
    /// </summary>
    public async Task<List<RoomReservationDto>> GetUpcomingReservationsClinic(int clinicId, int currentWeekNum)
    {
        var now = DateTime.UtcNow;
        var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        
        return await _db.RoomReservations.Include("Timeslot").IgnoreQueryFilters()
            .Where(x => x.ClinicId == clinicId && (x.WeekNum > currentWeekNum || x.WeekNum == currentWeekNum && x.Timeslot.Day > today || x.WeekNum == currentWeekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now)))
            .Select(x => new RoomReservationDto() { Id = x.Id, RoomId = x.RoomId, TimeslotId = x.TimeslotId, WeekNum = x.WeekNum, PractitionerId = x.PractitionerId, Status = x.ReservationStatus })
            .ToListAsync();
    }

    public async Task<List<RoomReservationDto>> GetUpcomingReservationsPractitioner(int practitionerId, int currentWeekNum)
    {
        var now = DateTime.UtcNow;
        var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        
        return await _db.RoomReservations.Include("Timeslot")
            .Where(x => x.PractitionerId == practitionerId && (x.WeekNum > currentWeekNum || x.WeekNum == currentWeekNum && x.Timeslot.Day > today || x.WeekNum == currentWeekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now)))
            .Select(x => new RoomReservationDto() { Id = x.Id, RoomId = x.RoomId, TimeslotId = x.TimeslotId, WeekNum = x.WeekNum, PractitionerId = x.PractitionerId, Status = x.ReservationStatus })
            .ToListAsync();
    }
}