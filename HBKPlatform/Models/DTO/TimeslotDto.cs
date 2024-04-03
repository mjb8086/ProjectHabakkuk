using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using HBKPlatform.Database;
using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    [DebuggerDisplay("WeekNum={WeekNum} Day={Day} Time={Time} Desc={Description}")]
    public class TimeslotDto: IComparable<TimeslotDto>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int PracticeId { get; set; }
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
                PracticeId = this.PracticeId,
                Day = this.Day,
                Time = this.Time,
                Duration = this.Duration,
                WeekNum = this.WeekNum
            };
        }

        public bool IsClash(TimeslotDto other)
        {
            return (other.WeekNum == this.WeekNum && other.Day == this.Day &&
                    (this.Time < other.Time.AddMinutes(other.Duration) &&
                     this.Time.AddMinutes(this.Duration) > other.Time));
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
            return x.Id == y.Id && x.Description == y.Description && x.PracticeId == y.PracticeId && x.Day == y.Day && x.Time.Equals(y.Time) && x.Duration == y.Duration && x.WeekNum == y.WeekNum;
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
                return x.Id == y.Id && x.Description == y.Description && x.PracticeId == y.PracticeId && x.Day == y.Day && x.Time.Equals(y.Time) && x.Duration == y.Duration && x.WeekNum == y.WeekNum;
            }

            public int GetHashCode(TimeslotDto obj)
            {
                return HashCode.Combine(obj.Id, obj.Description, obj.PracticeId, (int)obj.Day, obj.Time, obj.Duration, obj.WeekNum);
            }
        }

        public static IEqualityComparer<TimeslotDto> TimeslotDtoComparer { get; } = new TimeslotDtoEqualityComparer();

        public static TimeslotDto FromDbTimeslot(Timeslot timeslot)
        {
            return new()
            {
                Description = timeslot.Description,
                Id = timeslot.Id,
                Day = timeslot.Day,
                Time = timeslot.Time
            };
        }

    }
}