using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    // this is probably for the cutter too
    
    [DebuggerDisplay("WeekNum={WeekNum} Day={Day} Time={Time} Desc={Description}")]
    public class TimeslotDto: IComparable<TimeslotDto>
    {
        public int TimeslotIdx { get; set; }
        public string Description { get; set; }
        public Enums.Day Day { get; set; }
        public TimeOnly Time { get; set; }
        [Range(10,300)]
        public int DurationMinutes { get; set; }
        public int WeekNum { get; set; }

        /// <summary>
        /// Instantiates a duplicate of the current timeslotDto.
        /// </summary>
        /// <returns>A clone of the current timeslot with its members populated</returns>
        public TimeslotDto Clone()
        {
            return new TimeslotDto()
            {
                TimeslotIdx = this.TimeslotIdx,
                Description = this.Description,
                Day = this.Day,
                Time = this.Time,
                DurationMinutes = this.DurationMinutes,
                WeekNum = this.WeekNum
            };
        }

        public bool IsClash(TimeslotDto other)
        {
            return (other.WeekNum == this.WeekNum && other.Day == this.Day &&
                    (this.Time < other.Time.AddMinutes(other.DurationMinutes) &&
                     this.Time.AddMinutes(this.DurationMinutes) > other.Time));
        }
    
        public bool IsNotClashAny(List<TimeslotDto> others)
        {
            return !others.Any(x => x.IsClash(this));
        }
        
        /// <summary>
        /// this better bloody work, wtf is a sealed class?
        /// </summary>
        public bool Equals(TimeslotDto y)
        {
            var x = this;
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.TimeslotIdx == y.TimeslotIdx && x.Description == y.Description && x.Day == y.Day && x.Time.Equals(y.Time) && x.DurationMinutes == y.DurationMinutes && x.WeekNum == y.WeekNum;
        }

        public int CompareTo(TimeslotDto? next)
        {
            if (next == null)
            {
                return 1;
            }
            else if (next.WeekNum < this.WeekNum)
            {
                return 1;
            }
            else if (next.WeekNum > this.WeekNum)
            {
                return -1;
            }
            else if (next.WeekNum == this.WeekNum && next.Day < this.Day)
            {
                return 1;
            }
            else if (next.WeekNum == this.WeekNum && next.Day > this.Day)
            {
                return -1;
            }
            else if (next.WeekNum == this.WeekNum && next.Day == this.Day && next.Time < this.Time)
            {
                return 1;
            }
            else if (next.WeekNum == this.WeekNum && next.Day == this.Day && next.Time > this.Time)
            {
                return -1;
            }
            return 0;
        }

        private sealed class TimeslotDtoEqualityComparer : IEqualityComparer<TimeslotDto>
        {
            public bool Equals(TimeslotDto x, TimeslotDto y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.TimeslotIdx == y.TimeslotIdx && x.Description == y.Description && x.Day == y.Day && x.Time.Equals(y.Time) && x.DurationMinutes == y.DurationMinutes && x.WeekNum == y.WeekNum;
            }

            public int GetHashCode(TimeslotDto obj)
            {
                return HashCode.Combine(obj.TimeslotIdx, obj.Description, (int)obj.Day, obj.Time, obj.DurationMinutes, obj.WeekNum);
            }
        }

        public static IEqualityComparer<TimeslotDto> TimeslotDtoComparer { get; } = new TimeslotDtoEqualityComparer();

    }
}