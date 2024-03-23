namespace HBKPlatform.Models.View.MyND.RoomReservation;

public class ConfirmReservation
{
    public string ProspectiveDateTime { get; set; }
    public string RoomDetails { get; set; }
    public int RoomId { get; set; }
    public int WeekNum { get; set; }
    public int TimeslotId { get; set; }
}