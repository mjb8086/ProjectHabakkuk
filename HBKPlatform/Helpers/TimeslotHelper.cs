using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers
{
    public static class TimeslotHelper
    {
        public const int DEFAULT_DURATION_MINUTES = 5;
        public static readonly TimeOnly DEFAULT_START = new (08, 00, 00);
        public static readonly TimeOnly DEFAULT_END = new (19, 00, 00);
    
        public static List<Timeslot> GenerateDefaultTimeslots(TimeOnly startTime, TimeOnly maxTime, int duration = DEFAULT_DURATION_MINUTES)
        {
            var time = startTime;
            var timeslots = new List<Timeslot>();
            var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
            foreach (var day in days)
            {
                while (time <= maxTime)
                {
                    timeslots.Add(new Timeslot() { Description = $"{day} {time.ToShortTimeString()}", Day = day, Time = time, Duration = duration});
                    time = time.AddMinutes(duration);
                }

                time = startTime;
            }

            return timeslots;
        }
        
        /// <summary>
        /// Get a list of timeslots in the future, from this week day until the upper bound of the BookingAdvanceWeeks
        /// value. Each Ts DTO will have its weekNum field populated.
        /// </summary>
        public static SortedSet<TimeslotDto> GetPopulatedFutureTimeslots(DateTime now, List<TimeslotDto> allTimeslots, string dbStartDate, int bookingAdvance)
        {
            var thisWeek = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
            var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
            var nowTime = new TimeOnly(now.Hour, now.Minute, now.Second);

            var futureTs = new SortedSet<TimeslotDto>();

            var maxWeek = thisWeek + bookingAdvance;
            var currentWeekNum = thisWeek;
            while (currentWeekNum < maxWeek)
            {
                foreach (var ts in allTimeslots)
                {
                    var newTs = ts.Clone();
                    // split the timeslots at NOW, half will be this week, the preceding will be 'shifted' to the final week
                    if (currentWeekNum == thisWeek && (ts.Day < today || ts.Day == today && ts.Time < nowTime))
                    {
                        newTs.WeekNum = maxWeek;
                    }
                    else 
                    {
                        newTs.WeekNum = currentWeekNum;
                    } 
                    newTs.Description = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, newTs));
                    futureTs.Add(newTs);
                }

                currentWeekNum++;
            }

            return futureTs;
        }
    }
}