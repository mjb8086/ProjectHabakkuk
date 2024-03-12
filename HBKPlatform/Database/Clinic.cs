using System.ComponentModel.DataAnnotations;

namespace HBKPlatform.Database
{
    /// <summary>
    /// Clinic entity. Store details for each customer clinic to be displayed on its homepage
    /// as well as some of our business data like the licence status. Analogous to a tenant.
    /// </summary>
    public class Clinic : HbkBaseEntity
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
        public virtual ICollection<Practitioner> Practitioners { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
