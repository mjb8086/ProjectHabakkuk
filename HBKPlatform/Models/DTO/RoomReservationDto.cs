using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class RoomReservationDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int ClinicId { get; set; }
    public int PractitionerId { get; set; }
    public int StartTick { get; set; }
    public int EndTick { get; set; }
    
    public string PracticeNote { get; set; }
    public int WeekNum { get; set; }
    public Enums.ReservationStatus Status { get; set; }
}