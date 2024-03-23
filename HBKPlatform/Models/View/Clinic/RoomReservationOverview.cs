using HBKPlatform.Models.DTO;

namespace HBKPlatform.Models.View.Clinic;

public class RoomReservationOverview
{
    public List<RoomReservationLite> Requested { get; set; }
    public List<RoomReservationLite> Approved { get; set; }
    public List<RoomReservationLite> Denied { get; set; }
}