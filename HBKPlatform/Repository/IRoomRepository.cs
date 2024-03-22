using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IRoomRepository
{
    public Task Create(RoomDto room);
    public Task Update(RoomDto room);
    public Task<RoomDto> GetRoom(int roomId);
    public Task<List<RoomLite>> GetClinicRoomsLite(int clinicId);
    public Task<List<RoomDto>> GetRoomsAvailableForBooking();
}