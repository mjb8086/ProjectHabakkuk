using HBKPlatform.Globals;

namespace HBKPlatform.Models;

public class AvailabilityModel
{
    public string WeekStr { get; set; }
    public int WeekNum { get; set; }
    public Dictionary<Enums.Day, AvailabilityTuple> DailyTimeslotLookup { get; set; }
}

public class AvailabilityTuple
{
    public  int TimeslotId { get; set; }
    public bool IsAvailable { get; set; }
}
