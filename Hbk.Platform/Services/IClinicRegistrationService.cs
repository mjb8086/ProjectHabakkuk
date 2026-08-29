using Hbk.Common.Globals;
using Hbk.Database;
using Microsoft.AspNetCore.Identity;

namespace Hbk.Platform.Services;

public record ClinicRegistrationRequest(
    Enums.Title ManagerTitle,
    string ManagerForename,
    string ManagerSurname,
    string ManagerEmail,
    string Password,
    string ClinicName,
    string ClinicEmail,
    string Telephone,
    string? StreetAddress);

public record ClinicRegistrationResult(
    IdentityResult IdentityResult,
    User? User = null,
    int? TenancyId = null,
    int? ClinicId = null);

public interface IClinicRegistrationService
{
    Task<ClinicRegistrationResult> RegisterAsync(
        ClinicRegistrationRequest request,
        CancellationToken cancellationToken = default);
}
