using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND
{
    public class UpcomingAppointmentsView
    {
        public List<AppointmentDto> UpcomingAppointments { get; set; }
        public List<AppointmentDto> PastAppointments { get; set; }
        public List<AppointmentDto> RecentCancellations { get; set; }
    }
}