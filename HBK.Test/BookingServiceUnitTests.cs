using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBK.Test;

public class BookingServiceUnitTests
{
    public const int DEFAULT_DURATION = 30;
    
    private List<TimeslotDto> GenerateTimeslots() 
    {
        var timeslots = new List<TimeslotDto>();
        var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
        foreach (var day in days)
        {
            var time = new TimeOnly(09, 00, 00);
            var maxTime = new TimeOnly( 15, 00, 00);
            
            while (time <= maxTime)
            {
                timeslots.Add(new TimeslotDto() { Day = day, Time = time, Duration = DEFAULT_DURATION});
                time = time.AddMinutes(DEFAULT_DURATION);
            }
        }
        return timeslots;
    }
    
    
    
}