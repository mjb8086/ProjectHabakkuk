using System.Diagnostics;
using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

[DebuggerDisplay("WeekNum={WeekNum} Day={Day} Time={Time} Desc={Description}")]
public class TimeslotDto
{
    public int Id { get; set; }
    public string Description { get; set; }
    public int ClinicId { get; set; }
    public Enums.Day Day { get; set; }
    public TimeOnly Time { get; set; }
    public int Duration { get; set; }
    public int WeekNum { get; set; }

    /// <summary>
    /// Instantiates a duplicate of the current timeslotDto.
    /// </summary>
    /// <returns>A clone of the current timeslot with its members populated</returns>
    public TimeslotDto Clone()
    {
        return new TimeslotDto()
        {
            Id = this.Id,
            Description = this.Description,
            ClinicId = this.ClinicId,
            Day = this.Day,
            Time = this.Time,
            Duration = this.Duration,
            WeekNum = this.WeekNum
        };
    }
}