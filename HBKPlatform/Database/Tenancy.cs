using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBKPlatform.Globals;

namespace HBKPlatform.Database
{
    public class Tenancy
    {
        // Columns
        [Key, Column(Order = 1)]
        public int Id { get; set; }
        public string OrgName { get; set; }
        public string? OrgTagline { get; set; }
        public string? ContactEmail { get; set; }
        public Enums.LicenceStatus LicenceStatus { get; set; }
        public TenancyType Type { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    public enum TenancyType
    {
        NdAdmin, Practice, Clinic
    }
    
}