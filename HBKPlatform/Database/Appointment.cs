using System.ComponentModel.DataAnnotations;
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
        public int StartTick { get; set; }
        public int EndTick { get; set; }
//        public int Ticks { get; set; }
        [Range(0,5)]
        public int? EndAdjustment { get; set; }
        public int? RoomId { get; set; }
        public int? RoomReservationId { get; set; }
        public string? Note { get; set; }
        public int WeekNum { get; set; }
        public Enums.AppointmentStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    
        // EF Navigations
        public Client Client { get; set; }
        public Practitioner Practitioner { get; set; }
        public Treatment Treatment { get; set; }
        public Room? Room { get; set; }
        public RoomReservation? RoomReservation { get; set; }
    }
}