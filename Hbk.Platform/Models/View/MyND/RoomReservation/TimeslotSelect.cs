using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Models.View.MyND.RoomReservation;

public class TimeslotSelect
{
    public SortedSet<TimeslotDto> AvailableTimeslots { get; set; }
    public int RoomId { get; set; }
    public string RoomTitle { get; set; }
}