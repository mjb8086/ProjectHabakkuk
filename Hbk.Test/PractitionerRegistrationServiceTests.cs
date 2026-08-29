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

public class PractitionerRegistrationServiceTests
{
    [Fact]
    public async Task RegisterCreatesCompleteTrialPracticeAndPractitionerAccount()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includePractitionerRole: true);

        var result = await fixture.Service.RegisterAsync(ValidRequest());

        Assert.True(result.IdentityResult.Succeeded);
        Assert.NotNull(result.User);
        Assert.True(result.User.EmailConfirmed);
        Assert.Equal(-1, fixture.TenancyService.TenancyId);
        Assert.True(await fixture.UserManager.CheckPasswordAsync(result.User, "Password1!"));
        Assert.True(await fixture.UserManager.IsInRoleAsync(result.User, "Practitioner"));

        fixture.Context.ChangeTracker.Clear();
        var tenancy = await fixture.Context.Tenancies.SingleAsync();
        var practice = await fixture.Context.Practices
            .IgnoreQueryFilters()
            .Include(x => x.LeadPractitioner)
            .SingleAsync();
        var practitioner = await fixture.Context.Practitioners
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .SingleAsync();

        Assert.Equal(TenancyType.Practice, tenancy.Type);
        Assert.Equal(Enums.LicenceStatus.Trial, tenancy.LicenceStatus);
        Assert.Equal(tenancy.Id, practice.TenancyId);
        Assert.Equal(tenancy.Id, practitioner.TenancyId);
        Assert.Equal(tenancy.Id, practitioner.User.TenancyId);
        Assert.Equal(practitioner.Id, practice.LeadPractitionerId);
        Assert.Equal(practice.Id, practitioner.PracticeId);
        Assert.Equal("Alice Practitioner", practitioner.User.FullName);
        Assert.Equal(Enums.Sex.NotSpecified, practitioner.Sex);
    }

    [Fact]
    public async Task DuplicateEmailDoesNotCreateAnotherPracticeOrTenancy()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includePractitionerRole: true);
        Assert.True((await fixture.Service.RegisterAsync(ValidRequest())).IdentityResult.Succeeded);
        fixture.Context.ChangeTracker.Clear();

        var duplicate = await fixture.Service.RegisterAsync(ValidRequest() with
        {
            PracticeName = "Another Practice"
        });

        Assert.False(duplicate.IdentityResult.Succeeded);
        Assert.Contains(duplicate.IdentityResult.Errors, error => error.Code == nameof(IdentityErrorDescriber.DuplicateEmail));
        Assert.Equal(1, await fixture.Context.Tenancies.CountAsync());
        Assert.Equal(1, await fixture.Context.Practices.IgnoreQueryFilters().CountAsync());
        Assert.Equal(1, await fixture.Context.Practitioners.IgnoreQueryFilters().CountAsync());
        Assert.Equal(1, await fixture.Context.Users.CountAsync());
    }

    [Fact]
    public async Task MissingPractitionerRoleRollsBackAllRegistrationRecords()
    {
        await using var fixture = await RegistrationFixture.CreateAsync(includePractitionerRole: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Service.RegisterAsync(ValidRequest()));

        fixture.Context.ChangeTracker.Clear();
        Assert.Empty(await fixture.Context.Tenancies.ToListAsync());
        Assert.Empty(await fixture.Context.Practices.IgnoreQueryFilters().ToListAsync());
        Assert.Empty(await fixture.Context.Practitioners.IgnoreQueryFilters().ToListAsync());
        Assert.Empty(await fixture.Context.Users.ToListAsync());
        Assert.Equal(-1, fixture.TenancyService.TenancyId);
    }

    private static PractitionerRegistrationRequest ValidRequest() => new(
        Enums.Title.Dr,
        "Alice",
        "Practitioner",
        new DateOnly(1985, 4, 12),
        "alice@example.com",
        "Password1!",
        "Alice Health",
        "hello@alice-health.example",
        "01234 567890",
        "1 High Street",
        "Independent care");

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
            Service = new PractitionerRegistrationService(context, userManager, tenancyService);
        }

        private SqliteConnection Connection { get; }
        public ApplicationDbContext Context { get; }
        public TenancyService TenancyService { get; }
        public UserManager<User> UserManager { get; }
        public IPractitionerRegistrationService Service { get; }

        public static async Task<RegistrationFixture> CreateAsync(bool includePractitionerRole)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;
            var tenancyService = new TenancyService();
            var context = new ApplicationDbContext(options, new HttpContextAccessor(), tenancyService);
            await context.Database.EnsureCreatedAsync();

            if (includePractitionerRole)
            {
                context.Roles.Add(new IdentityRole
                {
                    Name = "Practitioner",
                    NormalizedName = "PRACTITIONER"
                });
                await context.SaveChangesAsync();
            }

            var store = new UserStore<User>(context);
            var identityOptions = Options.Create(new IdentityOptions());
            var userManager = new UserManager<User>(
                store,
                identityOptions,
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
