namespace HBKPlatform.Models.View;

public struct BookingCancel
{
    public int AppointmentId { get; set; }
    public string TreatmentTitle { get; set; }
    public string DateString { get; set; }
    public string PractitionerName { get; set; }
    public string ClientName { get; set; }
}