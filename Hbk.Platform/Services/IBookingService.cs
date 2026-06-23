using Hbk.Platform.Globals;
using Hbk.Platform.Models.DTO;
using Hbk.Platform.Models.View;
using Hbk.Platform.Models.View.MyND;

namespace Hbk.Platform.Services
{
    public interface IBookingService
    {
        public Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId);
        public Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId);
        public Task<List<AppointmentLite>> GetUpcomingAppointmentsForPractitioner(int pracId, bool liveOnly = true, string? dbStartDate = null);
        public Task<UpcomingAppointmentsView> GetMyNDUpcomingAppointmentsView();
        public Task<ClientUpcomingAppointmentsView> GetClientUpcomingAppointmentsView();
        public Task<BookingConfirm> GetBookingConfirmModel(PractitionerBookingFormModel model);
        public Task<BookingConfirm> GetBookingConfirmModel(int treatmentId, int timeslotId, int weekNum, int? roomResId = null, int? clientId = null);
        public Task<BookingConfirm> DoBookingClient(int treatmentId, int timeslotId, int weekNum);
        public Task<BookingConfirm> DoBookingPractitioner(int treatmentId, int timeslotId, int weekNum, int clientId, int? roomResId = null);
        public Task<BookClientTreatment> GetBookClientTreatmentView();
        public Task<BookingCancel> GetBookingCancelView(int appointmentId);
        public Task DoCancelBooking(int appointmentId, string reason, Enums.AppointmentStatus actioner);
    }
}