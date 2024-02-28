using HBKPlatform.Database;
using HBKPlatform.Globals;

namespace HBKPlatform.Helpers
{
    public class TimeslotHelper
    {
        public const int DEFAULT_DURATION = 30;
    
        public static List<Timeslot> GenerateDefaultTimeslots(Tenancy tenancy, int duration = DEFAULT_DURATION)
        {
            var timeslots = new List<Timeslot>();
            var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
            foreach (var day in days)
            {
                var time = new TimeOnly(08, 00, 00);
                var maxTime = new TimeOnly( 19, 00, 00);
            
                while (time <= maxTime)
                {
                    timeslots.Add(new Timeslot() { Tenancy = tenancy, Description = $"{day} {time.ToShortTimeString()}", Day = day, Time = time, Duration = duration});
                    time = time.AddMinutes(duration);
                }
            }

            return timeslots;
        }
    }
}