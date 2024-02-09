using System.Security.Claims;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Helpers;

public class AuthenticationHelper
{
    /// <summary>
    /// Return an array that will map the claim name to the claim value
    /// </summary>
    public static Claim[] GetClaimsForUser(UserDto userDto)
    {
        return new[]
        {
            new Claim("ClinicId", userDto.ClinicId.ToString()), 
            new Claim("LeadPractitionerId", userDto.PractitionerId?.ToString() ?? ""),
            new Claim("ClientId", userDto.ClientId?.ToString() ?? "")
        };
    }
}