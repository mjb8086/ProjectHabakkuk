using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Exceptions;

namespace HBKPlatform.Helpers
{
    /// <summary>
    /// HBKPlatform Date time helpers.
    /// 
    /// Author: Mark Brown
    /// Authored: 16/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class DateTimeHelper
    {
        public const string SHORTHAND_DATE_FORMAT = "dd/MM/yyyy";
        public const string FRIENDLY_DAY_FORMAT_NO_YEAR = "d MMM";
        public const string FRIENDLY_FULL_DATE_TIME = "dddd d MMMM yyyy, h:mm tt";
        public const int WEEKNUM_ORIGIN = 1;
        public const int TS_IDX_ORIGIN = 1;
        
        /// <summary>
        /// Get the DateTime representation from a timeslot, week number and DbStartDate.
        /// Note: WeekNumbers are 1-indexed. I.e., the week of dbStartDate will be counted as week 1
        /// Uses either the timeslotDto's weekNum or the parameter weekNum, with the parameter weekNum
        /// taking priority in the calculation.
        /// </summary>
        /// <exception cref="InvalidUserOperationException"></exception>
        public static DateTime FromTimeslot(string dbStartDate, TimeslotDto timeslot, int? weekNum = null)
        {
            if (string.IsNullOrEmpty(dbStartDate) || timeslot == null || (weekNum == null && timeslot.WeekNum < 1) || weekNum.HasValue && weekNum.Value < 1)
            {
                throw new InvalidUserOperationException("Inadequate parameters to produce DateTime.");
            }

            // subtract 1 from weekNum to account for the first week being week 1 (origin)
            return new DateTime(DateOnly.Parse(dbStartDate), timeslot.Time).AddDays((((weekNum ?? timeslot.WeekNum)-1) * 7) + (int)timeslot.Day);
        }
        
        
        public static DateTime FromTick(string dbStartDate, int tsIdx, int weekNum)
        {
            if (string.IsNullOrEmpty(dbStartDate) || weekNum < WEEKNUM_ORIGIN || tsIdx < TS_IDX_ORIGIN || tsIdx > TimeblockHelper.TICKS_PER_WEEK)
            {
                throw new InvalidUserOperationException("Invalid parameters, cannot produce a DateTime.");
            }

            // subtract 1 from weekNum to account for the first week being week 1 (origin)
            return new DateTime(DateOnly.Parse(dbStartDate), TimeblockHelper.GetTime(tsIdx)).AddDays((((weekNum-1) * 7) + (int)TimeblockHelper.GetDay(tsIdx)));
        }

        /// <summary>
        /// Get the current week number from the database start date
        /// </summary>
        public static int CurrentWeekNum(string dbStartDate)
        {
            return GetWeekNumFromDateTime(dbStartDate, DateTime.UtcNow);
        }
    
        public static int GetWeekNumFromDateTime(string dbStartDate, DateTime dateTime)
        {
            if (string.IsNullOrEmpty(dbStartDate) || dateTime == DateTime.MinValue)
            {
                throw new InvalidUserOperationException("Invalid parameters to produce WeekNum");
            }
            // add 1 to account for week 1
            return ((int)Math.Floor((dateTime - DateTime.Parse(dbStartDate)).TotalDays) / 7) + 1;
        }
    
        /// <summary>
        /// Verify that the start date falls on a Monday. Otherwise HBK can't guarantee other date time calculations.
        /// </summary>
        public static void ValidateDbStartDate(string dbStartDate)
        {
            if (DateTime.Parse(dbStartDate).DayOfWeek != DayOfWeek.Monday)
            {
                throw new InvalidConfigException("Database start date is not a Monday");
            }
        }

        /// <summary>
        /// Convert a .NET DayOfWeek to a HbkPlatform Day.
        /// .NET begins the week on a Sunday, we begin on a Monday. Thus some conversion is needed.
        /// </summary>
        public static Enums.Day ConvertDotNetDay(DayOfWeek day)
        {
            /*
             *       M T W T F S S
             * .NET  1 2 3 4 5 6 0  (USA)
             *  HBK  0 1 2 3 4 5 6   (UK)
             *  Damn Americanisations...
             */
            if (day == DayOfWeek.Sunday) return Enums.Day.Sunday;
            return (Enums.Day)day-1;
        }

        public static string GetFriendlyDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(FRIENDLY_FULL_DATE_TIME);
        }
        
        public static string GetIsoDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("s");
        }

        public static string GetDateRangeStringFromWeekNum(string dbStartDate, int weekNum, string? format = SHORTHAND_DATE_FORMAT)
        {
            if (weekNum <= 0) weekNum = 1;
            var startDate = DateTime.Parse(dbStartDate);
            startDate = startDate.AddDays(7 * (weekNum-1)); // week numbers are 0-indexed in practice...
            var endDate = startDate.AddDays(6);
            return $"{startDate.ToString(format)} - {endDate.ToString(format)}";
        }

        public static string GetDateRangeStringForThisWeek(string dbStartDate)
        {
            var now = DateTime.Now;
            var thisWeek = GetWeekNumFromDateTime(dbStartDate, now);
            var startDate = DateTime.Parse(dbStartDate);
            startDate = startDate.AddDays(7 * (thisWeek-1)); // week numbers are 0-indexed in practice...
            var endDate = startDate.AddDays(6);
        
            return $"{now.ToString(SHORTHAND_DATE_FORMAT)} - {endDate.ToString(SHORTHAND_DATE_FORMAT)}";
        }

    }
    public interface IDateTimeWrapper
    {
        /// <summary>
        /// Silly wrapper but saves massive headaches whilst unit testing. Returns the same value as DateTime.Now
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// Returns the same value as DateTime.UtcNow
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public class DateTimeWrapper : IDateTimeWrapper {}
}
