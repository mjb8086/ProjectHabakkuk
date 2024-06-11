using System.ComponentModel.DataAnnotations;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;

namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform TimeslotAvailability entity.
    /// Set per-week and indefinite availability per-timeslot on the Practitioner or Room entity.
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class TimeslotAvailability: HbkBaseEntity
    {
        [Range(0, TimeslotHelper.TIMESLOTS_PER_WEEK)]
        public int StartTick { get; set; }
        [Range(0, TimeslotHelper.TIMESLOTS_PER_WEEK)]
        public int EndTick { get; set; }
        [Range(0,5)]
        public int? EndAdjustment { get; set; }
        public int? PractitionerId { get; set; }
        public int? RoomId { get; set; }
        public int WeekNum { get; set; }
        public bool IsIndefinite { get; set; }
        public Enums.AvailabilityEntity Entity { get; set; }
        public Enums.TimeslotAvailability Availability { get; set; }
    
        // EF Navigations
        public virtual Practitioner? Practitioner { get; set; }
        public virtual Room? Room { get; set; }
    }
}