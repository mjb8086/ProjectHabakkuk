using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND.RoomReservation;

public class TimeslotSelect
{
    public List<TimeslotDateGroup> AvailableDates { get; set; } = [];
    public int RoomId { get; set; }
    public string RoomTitle { get; set; }
}

public class TimeslotDateGroup
{
    public DateOnly Date { get; set; }
    public List<TimeslotDto> Timeslots { get; set; } = [];
}
