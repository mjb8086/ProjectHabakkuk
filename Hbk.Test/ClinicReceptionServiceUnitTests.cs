using Hbk.Models.DTO;
using Hbk.Models.View.Clinic;
using Hbk.Platform.Services;
using Hbk.Platform.Services.Implementation;
using Moq;

namespace Hbk.Test;

public class ClinicReceptionServiceUnitTests
{
    [Fact]
    public async Task ReceptionModelContainsClinicNameAndStatistics()
    {
        const int clinicId = 7;
        var userService = new Mock<IUserService>();
        userService.Setup(x => x.GetClaimFromCookie("ClinicId")).Returns(clinicId);

        var cacheService = new Mock<ICacheService>();
        cacheService.Setup(x => x.GetClinicName(clinicId)).Returns("Clara Clinic");

        var roomService = new Mock<IRoomService>();
        roomService.Setup(x => x.GetClinicRooms()).ReturnsAsync([
            new RoomLite { Id = 1, Title = "Room 1" },
            new RoomLite { Id = 2, Title = "Room 2" },
            new RoomLite { Id = 3, Title = "Room 3" }
        ]);

        var reservationService = new Mock<IRoomReservationService>();
        reservationService.Setup(x => x.GetUpcomingReservationsClinic()).ReturnsAsync(new RoomReservationOverview
        {
            Requested = [new RoomReservationLite(), new RoomReservationLite()],
            Approved = [new RoomReservationLite()],
            Denied = [],
            Cancelled = []
        });

        var service = new ClinicReceptionService(
            userService.Object,
            cacheService.Object,
            roomService.Object,
            reservationService.Object);

        var model = await service.GetReceptionModel();

        Assert.Equal("Clara Clinic", model.ClinicName);
        Assert.Equal(3, model.NumRoomsRegistered);
        Assert.Equal(2, model.NumReservationRequests);
        Assert.Equal(1, model.NumApprovedReservations);
    }
}
