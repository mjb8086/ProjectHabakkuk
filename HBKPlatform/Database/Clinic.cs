using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database;

/// <summary>
/// Clinic entity. Store details for each customer clinic to be displayed on its homepage
/// as well as some of our business data like the licence status. Analogous to a tenant.
/// </summary>
public class Clinic : HbkBaseEntity
{
    public string OrgName { get; set; }
    public string? OrgTagline { get; set; }
    [DataType(DataType.MultilineText)]
    public string? StreetAddress { get; set; }
    public string Telephone { get; set; }
    [DataType(DataType.EmailAddress)] 
    public string EmailAddress { get; set; }
    public LicenceStatus LicenceStatus { get; set; }

    public Practitioner Practitioner { get; set; }
    public virtual ICollection<Client> Clients { get; set; }
    public ClinicHomepage ClinicHomepage { get; set; }
}

public enum LicenceStatus
{
    Trial,
    Active,
    Suspended
}