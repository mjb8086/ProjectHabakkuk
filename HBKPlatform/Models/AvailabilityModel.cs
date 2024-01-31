using HBKPlatform.Globals;

namespace HBKPlatform.Models;

public class AvailabilityModel
{
    public string WeekStr { get; set; }
    public int WeekNum { get; set; }
    public Dictionary<Enums.Day, List<AvailabilityLite>> DailyTimeslotLookup { get; set; }
}

public class AvailabilityLite
{
    public  int TimeslotId { get; set; }
    public string Description { get; set; }
    public bool IsAvailable { get; set; }
}
