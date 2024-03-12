namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform attribute entity.
/// Stores facilities and other details that may be available in rooms.
/// 
/// Author: Mark Brown
/// Authored: 12/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class Attribute: HbkBaseEntity
{
    public string Title { get; set; } 
    public string Description { get; set; }

    public virtual ICollection<RoomAttribute> RoomAttributes { get; set; }
}