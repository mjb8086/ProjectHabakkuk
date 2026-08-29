using Hbk.Common.Globals;
using Hbk.Common.Services;
using Hbk.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hbk.Platform.Services.Implementation;

public class ClinicRegistrationService(
    ApplicationDbContext db,
    UserManager<User> userManager,
    ITenancyService tenancyService) : IClinicRegistrationService
{
    public async Task<ClinicRegistrationResult> RegisterAsync(
        ClinicRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await userManager.FindByEmailAsync(request.ManagerEmail) is not null)
        {
            return Failure(new IdentityError
            {
                Code = nameof(IdentityErrorDescriber.DuplicateEmail),
                Description = "That email address is already registered."
            });
        }

        var previousTenancyId = tenancyService.TenancyId;
        IDbContextTransaction? transaction = null;

        try
        {
            if (db.Database.IsRelational())
            {
                transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            }

            var tenancy = new Tenancy
            {
                OrgName = request.ClinicName.Trim(),
                ContactEmail = request.ClinicEmail.Trim(),
                LicenceStatus = Enums.LicenceStatus.Trial,
                Type = TenancyType.Clinic,
                RegistrationDate = DateTime.UtcNow
            };

            db.Tenancies.Add(tenancy);
            await db.SaveChangesAsync(cancellationToken);
            tenancyService.SetTenancyId(tenancy.Id);

            var email = request.ManagerEmail.Trim();
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                PhoneNumber = string.Empty,
                PhoneNumberConfirmed = false,
                FullName = $"{request.ManagerTitle} {request.ManagerForename.Trim()} {request.ManagerSurname.Trim()}",
                Tenancy = tenancy
            };

            var createUserResult = await userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
            {
                await RollBackAsync(transaction, cancellationToken);
                return new ClinicRegistrationResult(createUserResult);
            }

            var clinic = new Clinic
            {
                EmailAddress = request.ClinicEmail.Trim(),
                Telephone = request.Telephone.Trim(),
                StreetAddress = NullIfWhiteSpace(request.StreetAddress),
                ManagerUser = user,
                Tenancy = tenancy
            };

            db.Clinics.Add(clinic);
            await db.SaveChangesAsync(cancellationToken);

            var roleResult = await userManager.AddToRoleAsync(user, "ClinicManager");
            if (!roleResult.Succeeded)
            {
                await RollBackAsync(transaction, cancellationToken);
                return new ClinicRegistrationResult(roleResult);
            }

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new ClinicRegistrationResult(
                IdentityResult.Success,
                user,
                tenancy.Id,
                clinic.Id);
        }
        catch
        {
            await RollBackAsync(transaction, cancellationToken);
            throw;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }

            tenancyService.SetTenancyId(previousTenancyId);
        }
    }

    private static ClinicRegistrationResult Failure(params IdentityError[] errors) =>
        new(IdentityResult.Failed(errors));

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static async Task RollBackAsync(
        IDbContextTransaction? transaction,
        CancellationToken cancellationToken)
    {
        if (transaction is not null)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}
