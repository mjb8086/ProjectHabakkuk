using Hbk.Common.Services.Implementation;
using Hbk.Common.Globals;
using Hbk.Database;
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
}
