using HBKPlatform.Database;
using HBKPlatform.Exceptions;
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
    
    
    /// <summary>
    /// Warning: Cross-Tenancy. Get a room reservation. Works across tenancies because it is used by Clash checking
    /// by the clinic.
    /// </summary>
    public async Task<RoomReservationDto> GetReservationAnyTenancy(int roomResId)
    {
        return await _db.RoomReservations.IgnoreQueryFilters()
                   .Select(x => new RoomReservationDto()
                   {
                       Id = x.Id, RoomId = x.RoomId, TimeslotId = x.TimeslotId, WeekNum = x.WeekNum,
                       PractitionerId = x.PractitionerId, Status = x.ReservationStatus
                   })
                   .FirstOrDefaultAsync(x => x.Id == roomResId) ??
               throw new IdxNotFoundException($"No reservation with Id {roomResId} exists");
    }
    
    public async Task<RoomReservationDto> GetReservation(int roomResId)
    {
        return await _db.RoomReservations.IgnoreQueryFilters()
                   .Select(x => new RoomReservationDto()
                   {
                       Id = x.Id, RoomId = x.RoomId, TimeslotId = x.TimeslotId, WeekNum = x.WeekNum,
                       PractitionerId = x.PractitionerId, Status = x.ReservationStatus
                   })
                   .FirstOrDefaultAsync(x => x.Id == roomResId) ??
               throw new IdxNotFoundException($"No reservation with Id {roomResId} exists");
    }

    /// <summary>
    /// Check whether or not an Approved or Booked reservation exists for the roomId across all tenancies.
    /// Will disregard Requested or Cancelled reservations - they are prospective and the clinic may choose to book any
    /// of the requests on the same week and timeslot. First come first served.
    /// </summary>
    public async Task<bool> CheckForExistingReservationAnyTenant(int weekNum, int timeslotId, int roomId)
    {
        return await _db.RoomReservations.IgnoreQueryFilters()
            .AnyAsync(x => x.RoomId == roomId && x.TimeslotId == timeslotId && x.WeekNum == weekNum && 
                           (x.ReservationStatus == Enums.ReservationStatus.Booked || 
                            x.ReservationStatus == Enums.ReservationStatus.Approved));
    }
    
    /// <summary>
    /// Check that on the same Timeslot and WeekNum there are no other Booked reservations for the room Id
    /// Excludes the current reservationId because that is our reservation and therefore not a double booking
    /// </summary>
    public async Task<bool> CheckForDoubleBookingAnyTenant(int weekNum, int timeslotId, int roomId, int currentResId)
    {
        return await _db.RoomReservations.IgnoreQueryFilters()
            .AnyAsync(x => x.RoomId == roomId && x.TimeslotId == timeslotId && x.WeekNum == weekNum && x.Id != currentResId &&
                           (x.ReservationStatus == Enums.ReservationStatus.Booked));
    }
}