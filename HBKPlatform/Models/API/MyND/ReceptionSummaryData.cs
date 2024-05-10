using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.API.MyND;

public class ReceptionSummaryData
{
    public List<AppointmentDto> UpcomingAppointments { get; set; }
    public List<AppointmentDto> RecentCancellations { get; set; }
    public List<ClientRecordLite> PriorityItems { get; set; }
    public int NumClientsRegistered { get; set; } 
    public int NumAppointmentsCompleted { get; set; } 
}