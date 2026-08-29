using Hbk.Common.Globals;
using Hbk.Common.Services;
using Hbk.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hbk.Platform.Services.Implementation;

public class PractitionerRegistrationService(
    ApplicationDbContext db,
    UserManager<User> userManager,
    ITenancyService tenancyService) : IPractitionerRegistrationService
{
    public async Task<PractitionerRegistrationResult> RegisterAsync(
        PractitionerRegistrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
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
                OrgName = request.PracticeName.Trim(),
                OrgTagline = NullIfWhiteSpace(request.PracticeTagline),
                ContactEmail = request.PracticeEmail.Trim(),
                LicenceStatus = Enums.LicenceStatus.Trial,
                Type = TenancyType.Practice,
                RegistrationDate = DateTime.UtcNow
            };

            db.Tenancies.Add(tenancy);
            await db.SaveChangesAsync(cancellationToken);
            tenancyService.SetTenancyId(tenancy.Id);

            var email = request.Email.Trim();
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                PhoneNumber = string.Empty,
                PhoneNumberConfirmed = false,
                FullName = $"{request.Forename.Trim()} {request.Surname.Trim()}",
                Tenancy = tenancy
            };

            var createUserResult = await userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
            {
                await RollBackAsync(transaction, cancellationToken);
                return new PractitionerRegistrationResult(createUserResult);
            }

            var practitioner = new Practitioner
            {
                Title = request.Title,
                Forename = request.Forename.Trim(),
                Surname = request.Surname.Trim(),
                DateOfBirth = request.DateOfBirth,
                Sex = Enums.Sex.NotSpecified,
                User = user,
                Tenancy = tenancy
            };

            var practice = new Practice
            {
                Description = request.PracticeName.Trim(),
                EmailAddress = request.PracticeEmail.Trim(),
                Telephone = request.Telephone.Trim(),
                StreetAddress = NullIfWhiteSpace(request.StreetAddress),
                Practitioners = [practitioner],
                Tenancy = tenancy
            };

            db.Practices.Add(practice);
            await db.SaveChangesAsync(cancellationToken);

            practice.LeadPractitioner = practitioner;
            await db.SaveChangesAsync(cancellationToken);

            var roleResult = await userManager.AddToRoleAsync(user, "Practitioner");
            if (!roleResult.Succeeded)
            {
                await RollBackAsync(transaction, cancellationToken);
                return new PractitionerRegistrationResult(roleResult);
            }

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return new PractitionerRegistrationResult(
                IdentityResult.Success,
                user,
                tenancy.Id,
                practice.Id,
                practitioner.Id);
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

    private static PractitionerRegistrationResult Failure(params IdentityError[] errors) =>
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
