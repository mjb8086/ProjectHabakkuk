using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBKPlatform.Globals;

namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform Timeslot entity.
/// 
/// Author: Mark Brown
/// Authored: 03/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class Timeslot: HbkBaseEntity
{
    public int ClinicId { get; set; }
    public Enums.Day Day { get; set; }
    public TimeOnly Time { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; }
    
    // EF Navigations
    public Clinic Clinic { get; set; }
}