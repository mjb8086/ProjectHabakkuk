using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services
{
    public interface IBookingService
    {
        public Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId);
        public Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId);

        public Task<List<AppointmentLite>> GetUpcomingAppointmentsForPractitioner(int pracId, int currentWeekNum,
            int currentTick, bool liveOnly = true);
        public Task<ClientUpcomingAppointmentsView> GetClientUpcomingAppointmentsView();
        public Task<BookingConfirm> GetBookingConfirmModel(PractitionerBookingFormModel model);
        public Task<BookingConfirm> GetBookingConfirmModel(int treatmentId, int timeslotId, int weekNum, int? roomResId = null, int? clientId = null);
        public Task<BookingConfirm> DoBookingClient(AppointmentRequestDto appointmentReq);
        public Task<BookClientTreatment> GetBookClientTreatmentView();
        public Task<BookingCancel> GetBookingCancelView(int appointmentId);
        public Task DoCancelBooking(int appointmentId, string reason, Enums.AppointmentStatus actioner);
    }
}