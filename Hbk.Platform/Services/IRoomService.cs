using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Services;

public interface IRoomService
{
    public Task Create(RoomDto room);
    public Task Update(RoomDto room);
    public Task<RoomDto> GetRoom(int roomId);
    public Task<List<RoomLite>> GetClinicRooms();
    public Task<List<RoomDto>> GetRoomsForBooking();
}