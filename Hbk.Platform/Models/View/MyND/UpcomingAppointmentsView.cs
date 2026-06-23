using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Models.View.MyND
{
    public class UpcomingAppointmentsView
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; }
        public List<AppointmentDto> PastAppointments { get; set; }
        public List<AppointmentDto> RecentCancellations { get; set; }
    }
}