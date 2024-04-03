using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database
{
    /// <summary>
    /// Store practice data - the umbrella legal entity under which a practitioner practices
    /// Includes clients, practitioners, and prac address/ contact details
    /// </summary>
    public class Practice : HbkBaseEntity
    {
        // Columns
        public string Description { get; set; }
        [DataType(DataType.MultilineText)]
        public string? StreetAddress { get; set; }
        public string Telephone { get; set; }
        [DataType(DataType.EmailAddress)] 
        public string EmailAddress { get; set; }
    
        public int? LeadPractitionerId { get; set; }
    
        // EF Navigations
        public Practitioner LeadPractitioner { get; set; }
        public ICollection<Practitioner> Practitioners { get; set; }
        public ICollection<Client> Clients { get; set; }
    }
}
