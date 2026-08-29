using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND
{
    public class ReceptionModel
    {
        public string PracticeName { get; set; } = string.Empty;
        public int NumUnreadMessages { get; set; }
        public int NumClientsRegistered { get; set; }
        public int NumAppointmentsCompleted { get; set; }
        public List<AppointmentLite> RecentBookings { get; set; } = [];
        public List<AppointmentLite> RecentCancellations { get; set; } = [];
        public List<ClientRecordLite> PriorityItems { get; set; } = [];
    }
}
