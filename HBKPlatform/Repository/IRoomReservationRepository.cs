using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IRoomReservationRepository
{
    public Task Create(RoomReservationDto reservation);
    public Task UpdateStatusClinic(int reservationId, Enums.ReservationStatus status, string? clinicNote);
    public Task UpdateStatusPractitioner(int reservationId, Enums.ReservationStatus status);
}