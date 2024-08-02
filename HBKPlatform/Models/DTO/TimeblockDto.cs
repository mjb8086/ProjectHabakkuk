using System.Diagnostics;

namespace HBKPlatform.Models.DTO;

/// <summary>
/// Timeblock - a consolidated sequence of ticks. Used for Start/End times of available continuous ticks when booking.
/// </summary>
[DebuggerDisplay("StartTick={StartTick}, EndTick={EndTick}")]
public class TimeblockDto : IEquatable<TimeblockDto>
{
    public int StartTick { get; set; }
    public int EndTick { get; set; }
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
    public DateTime EndTime { get; set; } // todo: remove?
    public int WeekNum { get; set; }

    public bool IsValid()
    {
        return (Duration == EndTick - StartTick) && EndTick > StartTick && WeekNum > 0;
    }

    public override string ToString()
    {
        return $"StartTick={StartTick}, EndTick={EndTick}.";
    }

    public bool Equals(TimeblockDto? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartTime.Equals(other.StartTime) && Duration == other.Duration && EndTime.Equals(other.EndTime) && StartTick == other.StartTick && EndTick == other.EndTick && WeekNum == other.WeekNum;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TimeblockDto)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, Duration, EndTime, StartTick, EndTick, WeekNum);
    }
}