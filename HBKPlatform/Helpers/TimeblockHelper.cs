using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers
{
    public static class TimeblockHelper
    {
        public const int TICK_DURATION_MINUTES = 5;
        public const int TICKS_PER_HOUR = 60 / TICK_DURATION_MINUTES;
        public const int TICKS_PER_DAY = 24 * TICKS_PER_HOUR;
        public const int TICKS_PER_WEEK = 7 * TICKS_PER_DAY;
        
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
            var timeslots = (tsIdx-1) % TICKS_PER_DAY;
            var minutes = timeslots * TICK_DURATION_MINUTES;
            return new TimeOnly(minutes/60, minutes%60);
        }

        public static Enums.Day GetDay(int tsIdx)
        {
            // Adjust for 1-indexing
            return (Enums.Day) ((tsIdx-1) / TICKS_PER_DAY);
        }

        // HBK day
        public static int GetFirstTickOfTheDay(Enums.Day day)
        {
            return (TICKS_PER_DAY * (int) day) + 1;
        }
        
        // .NET day
        public static int GetFirstTickOfTheDay(DayOfWeek day)
        {
            return (TICKS_PER_DAY * (int) DateTimeHelper.ConvertDotNetDay(day)) + 1;
        }

        public static int GetCurrentTick(DateTime? now = null)
        {
            now ??= DateTime.UtcNow;
            var ticksSinceMidnight = (now.Value.Hour * 60 + now.Value.Minute) / TICK_DURATION_MINUTES;
            var firstTickOfTheDay = GetFirstTickOfTheDay(now.Value.DayOfWeek);
            return ticksSinceMidnight + firstTickOfTheDay;
        }

        public static int GetTickFromDayHourMin(Enums.Day day, int hour, int min)
        {
            return GetFirstTickOfTheDay(day) + hour * TICKS_PER_HOUR + min / TICK_DURATION_MINUTES;
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
            if (source.Count <= 1) return source;
            
            if (!isOrdered) source = source.OrderBy(x => x.WeekNum).ThenBy(x => x.StartTick).ToList();
            var newSource = new List<TimeblockDto>();
            
            int startIdx = 0;
            while(startIdx < source.Count-1)
            {
                newSource.Add(FlattenTwoTbsMaybe(source[startIdx],ref startIdx, source[(startIdx+1)..source.Count]));
                startIdx++; // when we run out of matches, advance the startIdx anyway to avoid an infinite loop
            }

            return newSource;
        }

        /// <summary>
        /// Recursive function that will 'flatten', i.e. merge consecutive timeblocks
        /// </summary>
        private static TimeblockDto FlattenTwoTbsMaybe(TimeblockDto me, ref int startIdx, List<TimeblockDto> myNeighbours)
        {
            if (myNeighbours.Count == 1 && me.EndTick == myNeighbours[0].StartTick)
            {
                me.EndTick = myNeighbours[0].EndTick; // override instance value, may need to create a new instance
            }
            else if (myNeighbours.Count == 1) // final block, not consecutive
            {
                //?
            }
            else if (me.EndTick == myNeighbours[0].StartTick)
            {
                me.EndTick = myNeighbours[0].EndTick; // override instance value, may need to create a new instance
                startIdx++;
                return FlattenTwoTbsMaybe(me, ref startIdx, myNeighbours[1..myNeighbours.Count]);
            }
            return me;
        }

        /// <summary>
        /// Takes a list of Timeblocks and populates the StartTime and EndTimes for each.
        /// </summary>
        public static List<TimeblockDto> PopulateStartEndTimes(this List<TimeblockDto> source, int weekNum, string dbStartDate)
        {
            foreach (var tb in source)
            {
                tb.StartTime = DateTimeHelper.FromTick(dbStartDate, tb.StartTick, weekNum);
                tb.EndTime = DateTimeHelper.FromTick(dbStartDate, tb.EndTick, weekNum);
            }

            return source;
        }

        ///
        /// Source is the bigger list, other is the list we are subtracting.
        /// Assumes that the lists are sorted.
        /// 
        public static List<TimeblockDto> Difference(this List<TimeblockDto> source, List<TimeblockDto> other)
        {
            var newSource = new List<TimeblockDto>();
            int otherLen = other.Count;
            int otherStartIdx = 0;
            
            if (other.Count < 1) return source;
            // TODO - move this op into the loop below
            newSource = source.FlattenTimeblocks();
            other = other.FlattenTimeblocks();
            
            int startTick = source.First().StartTick, endTick = 0;
            foreach (var sourceTb in source)
            {
                // if we exhaust the otherList, just add this source element to the new list
                if (otherStartIdx == otherLen-1)
                {
                    newSource.Add(sourceTb);
                    continue;
                }
                
                for (int i=otherStartIdx; i < otherLen; i++)
                {
                    // exit iteration if otherTb falls outside of sourceTb, we are done with the sourceTb
                    otherStartIdx = i;
                    if (other[i].StartTick > sourceTb.EndTick)
                    {
                        break; // NO
                    }
                    
                    if (other[i].EndTick > sourceTb.EndTick) // we have a problem, otherTb end falls outside of source end - just use end time of source block
                    {
                        endTick = sourceTb.EndTick;
                    }
/*                    else if (i < otherLen -1 && other[i].EndTick == other[i+1].StartTick) // TODO: merge consecutive TBs in the same loop
                    {
                    } */
                    else
                    {
                        endTick = other[i].StartTick;
                    }
                    newSource.Add(new TimeblockDto(){StartTick = startTick, EndTick = endTick}); 
                    startTick = other[i].EndTick;
                    
                    if (i == otherLen-1) // we are at the end 
                    {
                        endTick = sourceTb.EndTick;
                        newSource.Add(new TimeblockDto(){StartTick = startTick, EndTick = endTick}); 
                    }
                    
                }
            }
            return newSource;
        }
    }
}