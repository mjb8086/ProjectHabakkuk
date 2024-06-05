using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.Clinic;
using HBKPlatform.Models.View.MyND.RoomReservation;

namespace HBKPlatform.Services;

public interface IRoomReservationService
{
    public Task Create(int roomId, int weekNum, int timeslotId);
    public Task CancelAsPractitioner(int reservationId);
    public Task CancelAsClinic(int id);
    public Task<RoomReservationOverview> GetUpcomingReservationsClinic();
    public Task<ConfirmReservation> GetConfirmReservationView(int roomId, int weekNum, int timeslotId);
    public Task ApproveReservation(int id, string? note = null);
    public Task DenyReservation(int id, string? note = null);
    public Task ConfirmRoomBookingPractitioner(int id);
    public Task<TimeslotSelect> GetTimeslotSelectView(int roomId);
    public Task<MyReservations> GetUpcomingReservationsPractitioner();
    public Task<List<RoomReservationLite>> GetHeldReservationsPractitioner();
    public Task<RoomLite> GetRoomDetailsFromReservation(int roomResId);
    public Task<RoomReservationDto> GetReservation(int reservationId);
    public Task VerifyRoomReservationPractitioner(RoomReservationDto roomRes, AppointmentRequestDto appointmentReq);
}