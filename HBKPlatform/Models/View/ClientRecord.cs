using HBKPlatform.Globals;

namespace HBKPlatform.Models.View;

public class ClientRecord
{
    public string? AppointmentDetails { get; set; }
    public DateTime AppointmentDate { get; set; }
    public Enums.RecordVisibility Visibility { get; set; }
    public string Title { get; set; }
    public string NoteBody { get; set; }
}