using HBKPlatform.Globals;

namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform ClientRecord.
/// 
/// Author: Mark Brown
/// Authored: 03/01/2023
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class ClientRecord : HbkBaseEntity
{
    public int ClinicId { get; set; }
    public int ClientId { get; set; }
    public int? AppointmentId { get; set; }
    public Enums.RecordVisibility RecordVisibility { get; set; }
    public string Title { get; set; }
    public string NoteBody { get; set; }
    
    // EF Navigations
    public virtual Clinic Clinic { get; set; }
    public virtual Client Client { get; set; }
    public virtual Appointment Appointment { get; set; }
}