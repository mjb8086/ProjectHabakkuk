using HBKPlatform.Globals;
using HBKPlatform.Models.View.Clinic;
using HBKPlatform.Models.View.MyND.RoomReservation;

namespace HBKPlatform.Services;

public interface IRoomReservationService
{
    public Task Create(int roomId, int weekNum, int timeslotId);
    public Task CancelAsPractitioner(int reservationId);
    public Task CancelAsClinic(int reservationId);
    public Task<RoomReservationOverview> GetUpcomingReservationsClinic();
    public Task<ConfirmReservation> GetConfirmReservationView(int roomId, int weekNum, int timeslotId);
    public Task UpdateStatusClinic(int id, Enums.ReservationStatus status, string? note);
    public Task UpdateStatusPractitioner(int id, Enums.ReservationStatus status);
    public Task<TimeslotSelect> GetTimeslotSelectView(int roomId);
    public Task<MyReservations> GetUpcomingReservationsPractitioner();
}