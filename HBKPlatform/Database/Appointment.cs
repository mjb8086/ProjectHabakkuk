using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform appointment entity.
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class Appointment: HbkBaseEntity
    {
        public int ClientId { get; set; }
        public int PractitionerId { get; set; }
        public int TreatmentId { get; set; }
        public int TimeslotId { get; set; }
        public int Duration { get; set; }
        public int? FinalAdjustment { get; set; }
        public int? RoomId { get; set; }
        public int? RoomReservationId { get; set; }
        public string? Note { get; set; }
        public int WeekNum { get; set; }
        public Enums.AppointmentStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    
        // EF Navigations
        public virtual Client Client { get; set; }
        public virtual Practitioner Practitioner { get; set; }
        public virtual Treatment Treatment { get; set; }
        public virtual Timeslot Timeslot { get; set; }
        public virtual Room? Room { get; set; }
        public virtual RoomReservation? RoomReservation { get; set; }
    }
}