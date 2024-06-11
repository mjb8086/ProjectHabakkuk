using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers
{
    public static class TimeslotHelper
    {
        public const int TIMESLOT_DURATION_MINUTES = 5;
        public const int TIMESLOTS_PER_HOUR = 60 / TIMESLOT_DURATION_MINUTES;
        public const int TIMESLOTS_PER_DAY = 24 * TIMESLOTS_PER_HOUR;
        public const int TIMESLOTS_PER_WEEK = 7 * TIMESLOTS_PER_DAY;
        
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

        public static TimeOnly GetTime(int tsIdx)
        {
            // Adjust for 1-indexing
            var timeslots = (tsIdx-1) % TIMESLOTS_PER_DAY;
            var minutes = timeslots * TIMESLOT_DURATION_MINUTES;
            return new TimeOnly(minutes/60, minutes%60);
        }

        public static Enums.Day GetDay(int tsIdx)
        {
            // Adjust for 1-indexing
            return (Enums.Day) ((tsIdx-1) / TIMESLOTS_PER_DAY);
        }

        // HBK day
        public static int GetFirstTickOfTheDay(Enums.Day day)
        {
            return (TIMESLOTS_PER_DAY * (int) day) + 1;
        }
        
        // .NET day
        public static int GetFirstTickOfTheDay(DayOfWeek day)
        {
            return (TIMESLOTS_PER_DAY * (int) DateTimeHelper.ConvertDotNetDay(day)) + 1;
        }

        public static int GetCurrentTick(DateTime? now = null)
        {
            now ??= DateTime.UtcNow;
            var ticksSinceMidnight = (now.Value.Hour * 60 + now.Value.Minute) / TIMESLOT_DURATION_MINUTES;
            var firstTickOfTheDay = GetFirstTickOfTheDay(now.Value.DayOfWeek);
            return ticksSinceMidnight + firstTickOfTheDay;
        }

        public static int GetTickFromDayHourMin(Enums.Day day, int hour, int min)
        {
            return GetFirstTickOfTheDay(day) + hour * TIMESLOTS_PER_HOUR + min / TIMESLOT_DURATION_MINUTES;
        }

        public static bool IsTickRangeValid(int startTick, int endTick)
        {
            return endTick > startTick;
        }
        
        /// <summary>
        /// Merge all consecutive timeblocks in this list.
        /// </summary>
        public static List<TimeblockDto> FlattenTimeblocks(this List<TimeblockDto> source, bool isOrdered = false)
        {
            var count = source.Count();
            if (count <= 1) return source;
            
            if (!isOrdered) source = source.OrderBy(x => x.WeekNum).ThenBy(x => x.StartTick).ToList();
            var newSource = new List<TimeblockDto>();
            foreach (var tb in source)
            {
                newSource.Add(FlattenTwoTbsMaybe(tb, source.Slice(1, source.Count)));
            }

            return newSource;
        }

        /// <summary>
        /// Recursive function that will 'flatten', i.e. merge consecutive timeblocks
        /// </summary>
        private static TimeblockDto FlattenTwoTbsMaybe(TimeblockDto me, List<TimeblockDto> myNeighbours)
        {
            if (myNeighbours.Count < 1) return me;
            if (me.EndTick == myNeighbours[1].StartTick)
                FlattenTwoTbsMaybe(
                    new TimeblockDto()
                        { StartTick = me.StartTick, EndTick = myNeighbours[1].EndTick, WeekNum = me.WeekNum },
                    myNeighbours.Slice(1, myNeighbours.Count));
            return me;
        }

        /// <summary>
        /// Takes a list of Timeblocks and populates the StartTime and EndTimes for each.
        /// </summary>
        public static List<TimeblockDto> InsertDateRanges(this List<TimeblockDto> source, string DbStartDate)
        {
            
        }
    }
}