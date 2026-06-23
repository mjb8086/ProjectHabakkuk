using Hbk.Models.DTO;

namespace Hbk.Models.View.MyND.RoomReservation;

public class MyReservations
{
    public List<RoomReservationLite> Requested { get; set; }
    public List<RoomReservationLite> Approved { get; set; }
    public List<RoomReservationLite> Denied { get; set; }
    public List<RoomReservationLite> Cancelled { get; set; }
}