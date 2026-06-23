using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Models.View
{
    public class TimeslotSelectView
    {
        public string TreatmentName { get; set; }
        public int TreatmentId { get; set; }
        public List<TimeslotDto> AvailableTimeslots { get; set; }
    }
}