using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View.MyND.RoomReservation;

public class TimeslotSelect
{
    public SortedSet<TimeslotDto> AvailableTimeslots { get; set; }
    public int RoomId { get; set; }
    public string RoomTitle { get; set; }
}