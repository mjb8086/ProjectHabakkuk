using Microsoft.AspNetCore.Identity;

namespace HBKPlatform.Database
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        // Full name is optional on users with either Practitioner or Client entity associated.
        public string? FullName { get; set; }
        public int LoginCount { get; set; }
        public DateTime? LastLogin { get; set; }
        public int TenancyId {get; set; }
        public virtual Tenancy Tenancy { get;set;}
    }
}

