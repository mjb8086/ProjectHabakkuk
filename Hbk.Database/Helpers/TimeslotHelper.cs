using Hbk.Common.Globals;

namespace Hbk.Database.Helpers
{
    public static class TimeslotHelper
    {
        public const int DEFAULT_DURATION_MINUTES = 30; // ideally would be 5m but the ui sucks for that
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
        
        
    }
}