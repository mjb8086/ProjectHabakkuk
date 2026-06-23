using System.Security.Claims;
using Hbk.Models.DTO;

namespace Hbk.Platform.Helpers
{
    public class AuthenticationHelper
    {
        /// <summary>
        /// Return an array that will map the claim name to the claim value
        /// </summary>
        public static Claim[] GetClaimsForUser(UserDto userDto)
        {
            return new[]
            {
                // TODO: Reconsider this, just create non null claims instead
                new Claim("PracticeId", userDto.PracticeId?.ToString() ?? ""), 
                new Claim("ClinicId", userDto.ClinicId?.ToString() ?? ""), 
                new Claim("PractitionerId", userDto.PractitionerId?.ToString() ?? ""),
                new Claim("ClientId", userDto.ClientId?.ToString() ?? ""),
                new Claim("TenancyId", userDto.TenancyId.ToString())
            };
        }
    }
}