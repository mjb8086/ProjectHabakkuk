using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database;

/// <summary>
/// HBKPlatform clinic entity
/// A Clinic owns rooms available for Practitioners to rent
/// 
/// Author: Mark Brown
/// Authored: 18/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class Clinic : HbkBaseEntity
{
    [DataType(DataType.MultilineText)]
    public string? StreetAddress { get; set; }
    public string Telephone { get; set; }
    [DataType(DataType.EmailAddress)] 
    public string EmailAddress { get; set; }
    public string ManagerUserId { get; set; }
    public virtual User ManagerUser { get; set; }
    public ICollection<Room> Rooms { get; set; }
}