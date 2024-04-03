using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IRoomReservationRepository
{
    public Task Create(RoomReservationDto reservation);
    public Task UpdateStatusClinic(int reservationId, Enums.ReservationStatus status, string? clinicNote = null);
    public Task UpdateStatusPractitioner(int reservationId, Enums.ReservationStatus status);
    public Task<List<RoomReservationDto>> GetUpcomingReservationsPractitioner(int practitionerId, int currentWeekNum);
    public Task<List<RoomReservationDto>> GetUpcomingReservationsClinic(int clinicId, int currentWeekNum);
    public Task<RoomReservationDto> GetReservation(int roomResId);
    public Task<RoomReservationDto> GetReservationAnyTenancy(int roomResId);

    public Task<bool> CheckForClashingReservationAnyTenant(int weekNum, int timeslotId, int roomId);
    public Task<bool> CheckForDoubleBookingAnyTenant(int weekNum, int timeslotId, int roomId, int currentResId);
}