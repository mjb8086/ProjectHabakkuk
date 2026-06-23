using Hbk.Common.Helpers;
using Hbk.Models.DTO;

namespace Hbk.Models.Helpers
{
    public static class DtoHelpers
    {
        
        /// <summary>
        /// Get the DateTime representation from a timeslot, week number and DbStartDate.
        /// Note: WeekNumbers are 1-indexed. I.e., the week of dbStartDate will be counted as week 1
        /// Uses either the timeslotDto's weekNum or the explicit weekNum, with the explicit weekNum
        /// taking priority in the calculation.
        /// </summary>
        /// <exception cref="InvalidUserOperationException"></exception>
        public static DateTime FromTimeslot(string dbStartDate, TimeslotDto timeslot, int? weekNum = null)
        {
            if (string.IsNullOrEmpty(dbStartDate) || timeslot == null || (weekNum == null && timeslot.WeekNum < 1) || weekNum.HasValue && weekNum.Value < 1)
            {
                throw new InvalidOperationException("Inadequate parameters to produce DateTime.");
            }

            // subtract 1 from weekNum to account for the first week being week 1 (origin)
            return new DateTime(DateOnly.Parse(dbStartDate), timeslot.Time).AddDays((((weekNum ?? timeslot.WeekNum)-1) * 7) + (int)timeslot.Day);
        }
        public static List<TimeslotLite> ConvertTimeslotsToLite(string dbStartDate, IEnumerable<TimeslotDto> timeslots)
        {
            var lites = new List<TimeslotLite>();
            foreach (var timeslot in timeslots)
            {
                if (timeslot.WeekNum < 1 ) continue;
                lites.Add(new TimeslotLite()
                {
                    Id = timeslot.Id,
                    Description = string.IsNullOrWhiteSpace(timeslot.Description) ? DateTimeHelper.GetFriendlyDateTimeString(DtoHelpers.FromTimeslot(dbStartDate, timeslot)) : timeslot.Description,
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