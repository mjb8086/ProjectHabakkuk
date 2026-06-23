using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Models.View
{
    public class ClientUpcomingAppointmentsView
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; }
        public bool SelfBookingEnabled { get; set; }
    }
}