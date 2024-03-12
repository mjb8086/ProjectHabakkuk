namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform Room entity.
/// 
/// Author: Mark Brown
/// Authored: 12/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class Room : HbkBaseEntity
{
    // Columns
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Img { get; set; }
    public int ClinicId { get; set; }
    
    // EV Navigations
    public virtual Clinic Clinic { get; set; }
    public virtual ICollection<RoomAttribute> RoomAttributes { get; set; }
}