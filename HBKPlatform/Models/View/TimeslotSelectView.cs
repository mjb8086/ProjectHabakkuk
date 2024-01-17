using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View;

public class TimeslotSelectView
{
    public string TreatmentName { get; set; }
    public int TreatmentId { get; set; }
    public List<TimeslotDto> AvailableTimeslots { get; set; }
}