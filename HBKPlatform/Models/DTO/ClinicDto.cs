using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO;

public class ClinicDto
{
    public int Id { get; set; }
    public string OrgName { get; set; }
    public Enums.LicenceStatus LicenceStatus { get; set; }
    public string OrgEmail { get; set; }
    public string Telephone { get; set; }
    public string StreetAddress { get; set; }
}

public class ClinicRegistrationDto : ClinicDto
{
    public Enums.Title LeadManagerTitle { get; set; }
    public string LeadManagerForename { get; set; }
    public string LeadManagerSurname { get; set; }
    
    public string LeadManagerEmail { get; set; }
}

public class ClinicDetailsDto : ClinicDto
{
    public string LeadManagerFullName { get; set; }
    public string LeadManagerEmail { get; set; }
    public DateTime RegistrationDate { get; set; }
}