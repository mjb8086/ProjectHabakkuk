using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View.MyND.RoomReservation;

public class MyReservations
{
    public List<RoomReservationLite> Requested { get; set; }
    public List<RoomReservationLite> Approved { get; set; }
    public List<RoomReservationLite> Denied { get; set; }
}