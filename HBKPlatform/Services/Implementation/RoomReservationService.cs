using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class RoomReservationService(IRoomReservationRepository _roomResRepo, IUserService _userService): IRoomReservationService
{
    public async Task Create(int roomId, int weekNum, int timeslotId)
    {
        var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
        // todo: clash check here?
        await _roomResRepo.Create(new RoomReservationDto() {RoomId = roomId, PractitionerId = practitionerId, WeekNum = weekNum, TimeslotId = timeslotId});
    }

    public async Task UpdateStatusClinic(int id, Enums.ReservationStatus status, string? note)
    {
        await _roomResRepo.UpdateStatusClinic(id, status, note);
    }
    
    public async Task UpdateStatusPractitioner(int id, Enums.ReservationStatus status)
    {
        await _roomResRepo.UpdateStatusPractitioner(id, status);
    }

    public async Task ConfirmReservation(int roomId, int weekNum, int timeslotId)
    {
        // Just return the room data to allow prac to confirm
    }
}