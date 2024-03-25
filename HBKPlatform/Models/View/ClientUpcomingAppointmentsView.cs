using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View
{
    public class ClientUpcomingAppointmentsView
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; }
        public bool SelfBookingEnabled { get; set; }
    }
}