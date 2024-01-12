using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View;

public class MyNDUpcomingAppointmentsView
{
    public List<AppointmentDto> UpcomingAppointments { get; set; }
    public List<AppointmentDto> PastAppointments { get; set; }
}