using Hbk.Common.Globals;
using Hbk.Database;
using Microsoft.AspNetCore.Identity;

namespace Hbk.Platform.Services;

public record PractitionerRegistrationRequest(
    Enums.Title Title,
    string Forename,
    string Surname,
    DateOnly DateOfBirth,
    string Email,
    string Password,
    string PracticeName,
    string PracticeEmail,
    string Telephone,
    string? StreetAddress,
    string? PracticeTagline);

public record PractitionerRegistrationResult(
    IdentityResult IdentityResult,
    User? User = null,
    int? TenancyId = null,
    int? PracticeId = null,
    int? PractitionerId = null);

public interface IPractitionerRegistrationService
{
    Task<PractitionerRegistrationResult> RegisterAsync(
        PractitionerRegistrationRequest request,
        CancellationToken cancellationToken = default);
}
