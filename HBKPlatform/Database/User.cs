using Microsoft.AspNetCore.Identity;

namespace HBKPlatform.Database;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public DateTime LastLogin { get; set; }
}

