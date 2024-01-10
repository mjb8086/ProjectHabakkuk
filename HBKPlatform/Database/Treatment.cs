using HBKPlatform.Globals;

namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform treatment entity.
/// Any billable service will be represented in this table.
/// 
/// Author: Mark Brown
/// Authored: 03/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class Treatment : HbkBaseEntity
{
    public int ClinicId { get; set; }
    public Enums.TreatmentRequestability TreatmentRequestability { get; set; }
    public double Cost { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Img { get; set; }
    
    // EF Navigations
    public virtual Clinic Clinic { get; set; }
}