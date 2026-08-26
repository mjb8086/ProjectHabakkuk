using Hbk.Common.Services.Implementation;
using Hbk.Common.Globals;
using Hbk.Database;
using Hbk.Platform.Repository.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hbk.Test;

public class SqliteDatabaseTests
{
    [Fact]
    public async Task SqliteCreatesSchemaAndSupportsBulkUpdates()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"hbk-test-{Guid.NewGuid()}.db");

        try
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using var context = new ApplicationDbContext(
                options,
                new HttpContextAccessor(),
                new TenancyService());

            await context.Database.EnsureCreatedAsync();

            context.Timeslots.Add(new Timeslot
            {
                Day = Enums.Day.Monday,
                Time = new TimeOnly(9, 0),
                Duration = 30,
                Description = "Available"
            });
            await context.SaveChangesAsync();

            var updated = await context.Timeslots
                .ExecuteUpdateAsync(update => update.SetProperty(timeslot => timeslot.Description, "Booked"));

            Assert.Equal(1, updated);
            Assert.Equal("Booked", await context.Timeslots.Select(timeslot => timeslot.Description).SingleAsync());
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    [Fact]
    public async Task RoomBookingProjectionIncludesClinicName()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"hbk-test-{Guid.NewGuid()}.db");

        try
        {
            var tenancyService = new TenancyService();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using var context = new ApplicationDbContext(
                options,
                new HttpContextAccessor(),
                tenancyService);

            await context.Database.EnsureCreatedAsync();

            var tenancy = new Tenancy
            {
                OrgName = "Wolseley Street Clinic",
                LicenceStatus = Enums.LicenceStatus.Active,
                Type = TenancyType.Clinic,
                RegistrationDate = DateTime.UtcNow
            };
            context.Tenancies.Add(tenancy);
            await context.SaveChangesAsync();
            tenancyService.SetTenancyId(tenancy.Id);

            context.Rooms.Add(new Room
            {
                Title = "Consulting Room",
                Description = "Ground floor",
                PricePerUse = 50,
                Tenancy = tenancy,
                Clinic = new Clinic
                {
                    Telephone = "01234 567890",
                    EmailAddress = "clinic@example.com",
                    Tenancy = tenancy,
                    ManagerUser = new User
                    {
                        UserName = "manager@example.com",
                        Tenancy = tenancy
                    }
                }
            });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var room = Assert.Single(await new RoomRepository(context).GetRoomsAvailableForBooking());

            Assert.Equal("Wolseley Street Clinic", room.ClinicName);
        }
        finally
        {
            File.Delete(databasePath);
        }
    }
}
