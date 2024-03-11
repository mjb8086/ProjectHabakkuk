using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers
{
    public static class DtoHelpers
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

        /// <summary>
        /// Expensive operation - duplicates a source and target, iterates all members and ensures they are identical.
        /// Intended for unit tests, does not compare all members - only the essentials.
        /// </summary>
        public static bool CompareTsList(this List<TimeslotDto> source, List<TimeslotDto> target)
        {
            if (source.Count != target.Count) return false;
            
            var orderedSource = source.OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
            var orderedTarget = target.OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
            
            for (int i=0; i< orderedSource.Count; i++)
            {
                if(!orderedTarget[i].Equals(orderedSource[i])) return false;
            }
            return true;
        }
        
    }
}