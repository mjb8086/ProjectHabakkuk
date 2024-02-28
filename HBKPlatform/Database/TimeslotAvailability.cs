using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform TimeslotAvailability entity.
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class TimeslotAvailability: HbkBaseEntity
    {
        public int TimeslotId { get; set; }
        public int PractitionerId { get; set; }
        public int WeekNum { get; set; }
        public bool IsIndefinite { get; set; }
        public Enums.TimeslotAvailability Availability { get; set; }
        public int Interlude { get; set; }
    
        // EF Navigations
        public virtual Timeslot Timeslot { get; set; }
        public virtual Practitioner Practitioner { get; set; }
    }
}