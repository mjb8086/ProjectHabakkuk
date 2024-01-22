using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;

namespace HBKPlatform.Services;

public interface IBookingService
{
    public Task<List<TimeslotDto>> GetAllTimeslots();
    public Task<TimeslotDto> GetTimeslot(int timeslotId);
    public Task<TimeslotManagementView> GetTimeslotMgmtView();
    public Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId);
    public Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId);
    public Task<List<AppointmentDto>> GetUpcomingAppointmentsForPractitioner(int pracId);
    public Task<MyNDUpcomingAppointmentsView> GetMyNDUpcomingAppointmentsView();
    public Task<ClientUpcomingAppointmentsView> GetClientUpcomingAppointmentsView();
    public Task<BookingConfirm> GetBookingConfirmModel(int treatmentId, int timeslotId, int weekNum);
    public Task<BookingConfirm> DoBookingClient(int treatmentId, int timeslotId, int weekNum);
}