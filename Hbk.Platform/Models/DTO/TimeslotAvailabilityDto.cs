using Hbk.Platform.Globals;

namespace Hbk.Platform.Models.DTO
{
    public class TimeslotAvailabilityDto
    {
        public int TimeslotId { get; set; }
        public int WeekNum { get; set; }
        public Enums.TimeslotAvailability Availability { get; set; }
        public bool IsIndefinite { get; set; } 
    }
}