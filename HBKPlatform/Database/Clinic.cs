using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBKPlatform.Globals;

namespace HBKPlatform.Database;

/// <summary>
/// Clinic entity. Store details for each customer clinic to be displayed on its homepage
/// as well as some of our business data like the licence status. Analogous to a tenant.
/// </summary>
public class Clinic : HbkBaseEntity
{
    // Columns
    public string OrgName { get; set; }
    public string? OrgTagline { get; set; }
    [DataType(DataType.MultilineText)]
    public string? StreetAddress { get; set; }
    public string Telephone { get; set; }
    [DataType(DataType.EmailAddress)] 
    public string EmailAddress { get; set; }
    public Enums.LicenceStatus LicenceStatus { get; set; }
    public DateTime RegistrationDate { get; set; }
    
    public int? LeadPractitionerId { get; set; }
    
    // EF Navigations
    public Practitioner LeadPractitioner { get; set; }
    public virtual ICollection<Practitioner> Practitioners { get; set; }
    public virtual ICollection<Client> Clients { get; set; }
    public ClinicHomepage ClinicHomepage { get; set; }
}
