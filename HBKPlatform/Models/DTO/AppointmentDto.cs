namespace HBKPlatform.Models.DTO;

public class AppointmentDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int ClientId { get; set; }
    public int PractitionerId { get; set; }
    public int TreatmentId { get; set; }
    public int WeekNum { get; set; }
    public int TimeslotId { get; set; }
    public string Note { get; set; }
    public string DateString { get; set; }
    public string TimeString { get; set; }
    public string ClientName { get; set; }
    public string PractitionerName { get; set; }
    public string TreatmentTitle { get; set; }
}