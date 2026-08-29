using Hbk.Models.View.Clinic;

namespace Hbk.Platform.Services.Implementation;

public class ClinicReceptionService(
    IUserService userService,
    ICacheService cacheService,
    IRoomService roomService,
    IRoomReservationService roomReservationService) : IClinicReceptionService
{
    public async Task<ReceptionModel> GetReceptionModel()
    {
        var clinicId = userService.GetClaimFromCookie("ClinicId");
        var rooms = await roomService.GetClinicRooms();
        var reservations = await roomReservationService.GetUpcomingReservationsClinic();

        return new ReceptionModel
        {
            ClinicName = cacheService.GetClinicName(clinicId),
            NumRoomsRegistered = rooms.Count,
            NumReservationRequests = reservations.Requested.Count,
            NumApprovedReservations = reservations.Approved.Count
        };
    }
}
