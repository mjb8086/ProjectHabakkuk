using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class ClinicDto
{
    public int Id { get; set; }
    public string OrgName { get; set; }
    public string? OrgTagline { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string? StreetAddress { get; set; }
    public Enums.LicenceStatus LicenceStatus { get; set; }
    public int PractitionerId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public PracDetailsLite LeadPrac { get; set; }
}