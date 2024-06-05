using System.ComponentModel.DataAnnotations;
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
    public int ClinicId { get; set; }
    public int PractitionerId { get; set; }
    public int StartTick { get; set; }
    public int EndTick { get; set; }
//    public int Ticks { get; set; }
    [Range(0,5)]
    public int? EndAdjustment { get; set; }
    
    public string? PracticeNote { get; set; }
    public string? ClinicNote { get; set; }
    public string? CancellationReason { get; set; }
    public int WeekNum { get; set; }
    public Enums.ReservationStatus ReservationStatus { get; set; }
    
    // EF Navigations
    public Room Room { get; set; }
    public Clinic Clinic { get; set; }
    public Practitioner Practitioner { get; set; }
}