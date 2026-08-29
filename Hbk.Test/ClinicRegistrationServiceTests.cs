using Hbk.Common.Globals;
using Hbk.Common.Services.Implementation;
using Hbk.Database;
using Hbk.Platform.Services;
using Hbk.Platform.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Hbk.Test;

public class ClinicRegistrationServiceTests
{
    [Fact]
    public async Task RegisterCreatesCompleteTrialClinicAndManagerAccount()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includeClinicManagerRole: true);

        var result = await fixture.Service.RegisterAsync(ValidRequest());

        Assert.True(result.IdentityResult.Succeeded);
        Assert.NotNull(result.User);
        Assert.True(result.User.EmailConfirmed);
        Assert.Equal(-1, fixture.TenancyService.TenancyId);
        Assert.True(await fixture.UserManager.CheckPasswordAsync(result.User, "Password1!"));
        Assert.True(await fixture.UserManager.IsInRoleAsync(result.User, "ClinicManager"));

        fixture.Context.ChangeTracker.Clear();
        var tenancy = await fixture.Context.Tenancies.SingleAsync();
        var clinic = await fixture.Context.Clinics
            .IgnoreQueryFilters()
            .Include(x => x.ManagerUser)
            .SingleAsync();

        Assert.Equal(TenancyType.Clinic, tenancy.Type);
        Assert.Equal(Enums.LicenceStatus.Trial, tenancy.LicenceStatus);
        Assert.Equal("Clara Clinic", tenancy.OrgName);
        Assert.Equal(tenancy.Id, clinic.TenancyId);
        Assert.Equal(tenancy.Id, clinic.ManagerUser.TenancyId);
        Assert.Equal(clinic.ManagerUserId, result.User.Id);
        Assert.Equal("Ms Clara Manager", clinic.ManagerUser.FullName);
    }

    [Fact]
    public async Task DuplicateEmailDoesNotCreateAnotherClinicOrTenancy()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includeClinicManagerRole: true);
        Assert.True((await fixture.Service.RegisterAsync(ValidRequest())).IdentityResult.Succeeded);
        fixture.Context.ChangeTracker.Clear();

        var duplicate = await fixture.Service.RegisterAsync(ValidRequest() with
        {
            ClinicName = "Another Clinic"
        });

        Assert.False(duplicate.IdentityResult.Succeeded);
        Assert.Contains(duplicate.IdentityResult.Errors, error => error.Code == nameof(IdentityErrorDescriber.DuplicateEmail));
        Assert.Equal(1, await fixture.Context.Tenancies.CountAsync());
        Assert.Equal(1, await fixture.Context.Clinics.IgnoreQueryFilters().CountAsync());
        Assert.Equal(1, await fixture.Context.Users.CountAsync());
    }

    [Fact]
    public async Task MissingClinicManagerRoleRollsBackAllRegistrationRecords()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includeClinicManagerRole: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.RegisterAsync(ValidRequest()));

        fixture.Context.ChangeTracker.Clear();
        Assert.Empty(await fixture.Context.Tenancies.ToListAsync());
        Assert.Empty(await fixture.Context.Clinics.IgnoreQueryFilters().ToListAsync());
        Assert.Empty(await fixture.Context.Users.ToListAsync());
        Assert.Equal(-1, fixture.TenancyService.TenancyId);
    }

    private static ClinicRegistrationRequest ValidRequest() => new(
        Enums.Title.Ms,
        "Clara",
        "Manager",
        "clara@example.com",
        "Password1!",
        "Clara Clinic",
        "hello@clara-clinic.example",
        "01234 567890",
        "2 High Street");

    private sealed class RegistrationFixture : IAsyncDisposable
    {
        private RegistrationFixture(
            SqliteConnection connection,
            ApplicationDbContext context,
            TenancyService tenancyService,
            UserManager<User> userManager)
        {
            Connection = connection;
            Context = context;
            TenancyService = tenancyService;
            UserManager = userManager;
            Service = new ClinicRegistrationService(context, userManager, tenancyService);
        }

        private SqliteConnection Connection { get; }
        public ApplicationDbContext Context { get; }
        public TenancyService TenancyService { get; }
        public UserManager<User> UserManager { get; }
        public IClinicRegistrationService Service { get; }

        public static async Task<RegistrationFixture> CreateAsync(bool includeClinicManagerRole)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
            var tenancyService = new TenancyService();
            var context = new ApplicationDbContext(options, new HttpContextAccessor(), tenancyService);
            await context.Database.EnsureCreatedAsync();

            if (includeClinicManagerRole)
            {
                context.Roles.Add(new IdentityRole
                {
                    Name = "ClinicManager",
                    NormalizedName = "CLINICMANAGER"
                });
                await context.SaveChangesAsync();
            }

            var store = new UserStore<User>(context);
            var userManager = new UserManager<User>(
                store,
                Options.Create(new IdentityOptions()),
                new PasswordHasher<User>(),
                [new UserValidator<User>()],
                [new PasswordValidator<User>()],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                Mock.Of<IServiceProvider>(),
                NullLogger<UserManager<User>>.Instance);

            return new RegistrationFixture(connection, context, tenancyService, userManager);
        }

        public async ValueTask DisposeAsync()
        {
            UserManager.Dispose();
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
}
