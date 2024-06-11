namespace HBKPlatform.Models.DTO;

/// <summary>
/// Timeblock - a consolidated sequence of ticks. Used for Start/End times of available continuous ticks when booking.
/// </summary>
public class TimeblockDto
{
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
//    public DateTime EndTime { get; set; }
    public int StartTick { get; set; }
    public int EndTick { get; set; }
    public int WeekNum { get; set; }
}