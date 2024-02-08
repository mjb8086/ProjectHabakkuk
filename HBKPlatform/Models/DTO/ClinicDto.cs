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
}

// Additional fields used to view clinic details in MCP
public class ClinicDetailsDto : ClinicDto
{
    public string LeadPracFullName { get; set; }
}

// For use on initial registration only.
public class ClinicRegistrationDto : ClinicDto 
{
    public Enums.Title LeadPracTitle { get; set; }
    public string LeadPracForename { get; set; }
    public string LeadPracSurname { get; set; }
    public DateOnly LeadPracDOB { get; set; }
    public string LeadPracEmail { get; set; }

}