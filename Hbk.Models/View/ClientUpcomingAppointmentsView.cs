using Hbk.Models.DTO;

namespace Hbk.Models.View
{
    public class ClientUpcomingAppointmentsView
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; }
        public bool SelfBookingEnabled { get; set; }
    }
}