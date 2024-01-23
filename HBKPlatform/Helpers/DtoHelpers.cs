using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers;

public class DtoHelpers
{
    public static List<TimeslotLite> ConvertTimeslotsToLite(string dbStartDate, List<TimeslotDto> timeslots)
    {
        var lites = new List<TimeslotLite>();
        foreach (var timeslot in timeslots)
        {
            if (timeslot.WeekNum < 1 ) continue;
            lites.Add(new TimeslotLite()
            {
                Id = timeslot.Id,
                Description = string.IsNullOrWhiteSpace(timeslot.Description) ? DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslot)) : timeslot.Description,
                WeekNum = timeslot.WeekNum
            });
        }
        return lites;
    }

    public static List<TreatmentLite> ConvertTreatmentsToLite(IEnumerable<TreatmentDto> treatments)
    {
        var lites = new List<TreatmentLite>();
        foreach (var treatment in treatments)
        {
           lites.Add(new TreatmentLite()
           {
                Id = treatment.Id,
                Title = treatment.Title,
                Cost = treatment.Cost,
                Requestability = treatment.Requestability
           }); 
        }

        return lites;
    }
}