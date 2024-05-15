using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.API.MyND;

public class ReceptionSummaryData
{
    public List<AppointmentLite> UpcomingAppointments { get; set; }
    public List<AppointmentLite> RecentCancellations { get; set; }
    public List<ClientRecordLite> PriorityItems { get; set; }
    public List<RoomReservationLite> RoomReservations { get; set; }
    public int NumClientsRegistered { get; set; } 
    public int NumAppointmentsCompleted { get; set; } 
    public int AdditionalUpcoming { get; set; }
    public int AdditionalCancellations { get; set; }
}