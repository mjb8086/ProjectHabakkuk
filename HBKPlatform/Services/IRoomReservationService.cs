using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services;

public interface IRoomReservationService
{
    public Task Create(int roomId, int weekNum, int timeslotId);
    public Task ConfirmReservation(int roomId, int weekNum, int timeslotId);
    public Task UpdateStatusClinic(int id, Enums.ReservationStatus status, string? note);
    public Task UpdateStatusPractitioner(int id, Enums.ReservationStatus status);
}