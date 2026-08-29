namespace Hbk.Models.View.Clinic;

public class ReceptionModel
{
    public string ClinicName { get; set; } = string.Empty;
    public int NumRoomsRegistered { get; set; }
    public int NumReservationRequests { get; set; }
    public int NumApprovedReservations { get; set; }
}
