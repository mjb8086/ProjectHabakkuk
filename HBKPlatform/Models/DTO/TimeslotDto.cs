using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class TimeslotDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public int ClinicId { get; set; }
    public Enums.Day Day { get; set; }
    public TimeOnly Time { get; set; }
    public int Duration { get; set; }
}