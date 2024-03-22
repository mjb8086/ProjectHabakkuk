using HBKPlatform.Globals;

namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform room reservation entity
/// 
/// Author: Mark Brown
/// Authored: 22/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class RoomReservation : HbkBaseEntity
{
    public int RoomId { get; set; }
    public int PractitionerId { get; set; }
//    public int TreatmentId { get; set; }
    public int TimeslotId { get; set; }
    
    public string PracticeNote { get; set; }
    public string ClinicNote { get; set; }
    public string CancellationReason { get; set; }
    public int WeekNum { get; set; }
    public Enums.ReservationStatus ReservationStatus { get; set; }
    
    // EF Navigations
    public virtual Room Room { get; set; }
    public virtual Practitioner Practitioner { get; set; }
 //   public virtual Treatment Treatment { get; set; }
    public virtual Timeslot Timeslot { get; set; }
}