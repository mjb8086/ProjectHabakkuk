using System.ComponentModel.DataAnnotations;
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
    [Range(10,300)]
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

    public bool IsClash(TimeslotDto other)
    {
        if (other.WeekNum == this.WeekNum && other.Day == this.Day && other.Time == this.Time) return true;
        return (other.WeekNum == this.WeekNum && other.Day == this.Day &&
                (this.Time < other.Time.AddMinutes(other.Duration) &&
                 this.Time.AddMinutes(this.Duration) > other.Time));
    }

    private sealed class TimeslotDtoEqualityComparer : IEqualityComparer<TimeslotDto>
    {
        public bool Equals(TimeslotDto x, TimeslotDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.Description == y.Description && x.ClinicId == y.ClinicId && x.Day == y.Day && x.Time.Equals(y.Time) && x.Duration == y.Duration && x.WeekNum == y.WeekNum;
        }

        public int GetHashCode(TimeslotDto obj)
        {
            return HashCode.Combine(obj.Id, obj.Description, obj.ClinicId, (int)obj.Day, obj.Time, obj.Duration, obj.WeekNum);
        }
    }

    public static IEqualityComparer<TimeslotDto> TimeslotDtoComparer { get; } = new TimeslotDtoEqualityComparer();
}