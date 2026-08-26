using Hbk.Common.Globals;
using Hbk.Common.Helpers;
using Hbk.Models.DTO;
using Hbk.Platform.Repository;
using Hbk.Platform.Services;
using Hbk.Platform.Services.Implementation;
using Moq;

namespace Hbk.Test;

public class ClientRecordServiceUnitTests
{
    [Fact]
    public async Task ClientRecordModelsUseCreationDateAndReadableVisibility()
    {
        var created = new DateTime(2026, 8, 24, 14, 30, 0);
        var liteRecord = new ClientRecordLite
        {
            Id = 10,
            ClientId = 7,
            Title = "Consultation notes",
            Date = created,
            Visibility = Enums.RecordVisibility.PracOnly
        };
        var fullRecord = new FullClientRecordDto
        {
            Id = 10,
            ClientId = 7,
            Title = "Consultation notes",
            NoteBody = "Notes",
            DateCreated = created,
            Visibility = Enums.RecordVisibility.ClientAndPrac
        };

        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetClientName(7)).Returns("Edward Munster");

        var repository = new Mock<IRecordRepository>();
        repository.Setup(x => x.GetClientRecordsLite(7)).ReturnsAsync([liteRecord]);
        repository.Setup(x => x.GetRecord(10)).ReturnsAsync(fullRecord);

        var service = new ClientRecordService(
            new Mock<IUserService>().Object,
            cache.Object,
            repository.Object);

        var listModel = await service.GetClientRecords(7);
        var detailModel = await service.GetClientRecord(10, null);
        var expectedDate = DateTimeHelper.GetFriendlyDateTimeString(created);

        Assert.Equal(expectedDate, listModel.ClientRecordList[0].DisplayDate);
        Assert.Equal(expectedDate, detailModel.DisplayDateCreated);
        Assert.Equal("Practitioner only", liteRecord.Visibility.GetDisplayName());
        Assert.Equal("Client and practitioner", fullRecord.Visibility.GetDisplayName());
    }
}
