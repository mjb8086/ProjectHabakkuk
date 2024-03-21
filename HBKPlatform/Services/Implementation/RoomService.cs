using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

// Hello,
public class RoomService(IRoomRepository _roomRepo, IUserService _userService) : IRoomService // set a wake up call at 4am please... rooms 601-604. Thanks!
{
    public async Task Create(RoomDto room)
    {
        room.ClinicId = _userService.GetClaimFromCookie("ClinicId");
        await _roomRepo.Create(room);
    }
    
    public async Task Update(RoomDto room)
    {
        await _roomRepo.Update(room);
    }

    public async Task<List<RoomLite>> GetClinicRooms()
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        return await _roomRepo.GetClinicRoomsLite(clinicId);
    }

    public async Task<RoomDto> GetRoom(int roomId)
    {
        return await _roomRepo.GetRoom(roomId);
    }
    
}