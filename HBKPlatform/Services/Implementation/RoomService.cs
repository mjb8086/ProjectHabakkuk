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

    /// <summary>
    /// Get all a Clinic's rooms. Tenancy filters are in effect.
    /// </summary>
    public async Task<List<RoomLite>> GetClinicRooms()
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        return await _roomRepo.GetClinicRoomsLite(clinicId);
    }

    /// <summary>
    /// Get any room by ID, tenancy filters are in effect.
    /// </summary>
    public async Task<RoomDto> GetRoom(int roomId)
    {
        return await _roomRepo.GetRoom(roomId);
    }

    /// <summary>
    /// Get all rooms suitable for booking by a practitioner.
    /// Tenancy filters are NOT in effect.
    /// </summary>
    /// <returns></returns>
    public async Task<List<RoomDto>> GetRoomsForBooking()
    {
        return await _roomRepo.GetRoomsAvailableForBooking();
    }
    
}