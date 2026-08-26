using Hbk.Common.Globals;
using Hbk.Common.Services;
using Hbk.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Hbk.Test;

public class ApplicationDbContextTests
{
    [Fact]
    public async Task SaveChangesSetsCreationDateWhenDatabaseProviderHasNoDefault()
    {
        var tenancyService = new Mock<ITenancyService>();
        tenancyService.SetupGet(x => x.TenancyId).Returns(5);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"creation-date-{Guid.NewGuid()}")
            .Options;
        await using var context = new ApplicationDbContext(
            options,
            new HttpContextAccessor(),
            tenancyService.Object);
        var record = new ClientRecord
        {
            ClientId = 1,
            PractitionerId = 2,
            Title = "Consultation notes",
            NoteBody = "Notes",
            RecordVisibility = Enums.RecordVisibility.PracOnly
        };
        var beforeSave = DateTime.UtcNow;

        context.ClientRecords.Add(record);
        await context.SaveChangesAsync();
        var afterSave = DateTime.UtcNow;

        Assert.InRange(record.DateCreated, beforeSave, afterSave);
        Assert.Equal(5, record.TenancyId);
    }
}
