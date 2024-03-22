using HBKPlatform.Database;
using HBKPlatform.Globals;
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
                TimeslotId = reservation.TimeslotId,
                PractitionerId = reservation.PractitionerId,
                PracticeNote = reservation.PracticeNote,
                WeekNum = reservation.WeekNum,
                ReservationStatus = Enums.ReservationStatus.Requested
            });
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

    public async Task GetUpcomingReservationsClinic()
    {
        
    }

    public async Task GetUpcomingReservationsPractitioner(int currentWeekNum, int currentTimeslot)
    {
        
    }
}