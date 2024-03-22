namespace HBKPlatform.Models.DTO;

public class RoomReservationDto
{
    public int RoomId { get; set; }
    public int PractitionerId { get; set; }
    public int TimeslotId { get; set; }
    
    public string PracticeNote { get; set; }
    public int WeekNum { get; set; }
}