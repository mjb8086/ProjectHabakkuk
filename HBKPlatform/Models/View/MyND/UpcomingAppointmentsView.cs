using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View.MyND;

public class UpcomingAppointmentsView
{
    public List<AppointmentDto> UpcomingAppointments { get; set; }
    public List<AppointmentDto> PastAppointments { get; set; }
    public List<AppointmentDto> RecentCancellations { get; set; }
}