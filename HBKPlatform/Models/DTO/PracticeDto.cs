using HBKPlatform.Globals;

namespace HBKPlatform.Models.DTO
{
    public class PracticeDto
    {
        public int Id { get; set; }
        public string OrgName { get; set; }
        public string? OrgTagline { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string? StreetAddress { get; set; }
        public Enums.LicenceStatus LicenceStatus { get; set; }
        public int LeadPractitionerId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

// Additional fields used to view practice details in MCP
    public class PracticeDetailsDto : PracticeDto
    {
        public string LeadPractitionerFullName { get; set; }
    }

// For use on initial registration only.
    public class PracticeRegistrationDto : PracticeDto 
    {
        public Enums.Title LeadPracTitle { get; set; }
        public string LeadPracForename { get; set; }
        public string LeadPracSurname { get; set; }
        public DateTime LeadPracDOB { get; set; }
        public string LeadPracEmail { get; set; }

    }
}