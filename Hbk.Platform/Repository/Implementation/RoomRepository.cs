using Hbk.Common.Exception;
using Hbk.Database;
using Hbk.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hbk.Platform.Repository.Implementation;

public class RoomRepository (ApplicationDbContext _db): IRoomRepository
{
    
    ////////////////////////////////////////////////////////////////////////////////
    // NB: Query filters are in effect. These methods are suitable only for Clinics.
    ////////////////////////////////////////////////////////////////////////////////
    public async Task Create(RoomDto room)
    {
        await _db.AddAsync(new Room()
        {
            ClinicId = room.ClinicId,
            Description = room.Description,
            Title = room.Title,
            Img = room.Img,
            PricePerUse = room.PricePerUse
        });
        await _db.SaveChangesAsync();
    }
    
    public async Task Update(RoomDto room)
    {
        var dbRoom = await _db.Rooms.FirstOrDefaultAsync(x => room.Id == x.Id) 
                ?? throw new IdxNotFoundException($"RoomId {room.Id} not found in database.");
        dbRoom.Description = room.Description;
        dbRoom.Title = room.Title;
        dbRoom.Img = room.Img;
        dbRoom.PricePerUse = room.PricePerUse;
        await _db.SaveChangesAsync();
    }
    
    public async Task<List<RoomLite>> GetClinicRoomsLite(int clinicId)
    {
        return await _db.Rooms.Where(x => x.ClinicId == clinicId).Select(x => new RoomLite()
        {
            Id = x.Id,
            Title = x.Title
        }).ToListAsync();
    }
    
    public async Task<RoomDto> GetRoom(int roomId)
    {
        return await _db.Rooms.Where(x => x.Id == roomId).Select(RoomDtoProjection).FirstOrDefaultAsync() ??
               throw new IdxNotFoundException($"RoomId {roomId} does not exist.");
    }
    
    ////////////////////////////////////////////////////////////////////////////////
    // The following have no query filters. I.e. Access is not restricted by tenancy.
    // These may be used by Pracs to find rooms.
    ////////////////////////////////////////////////////////////////////////////////

    public async Task<List<RoomDto>> GetRoomsAvailableForBooking()
    {
        return await _db.Rooms.IgnoreQueryFilters().Select(RoomDtoProjection).ToListAsync();
    }

    ////////////////////////////////////////////////////////////////////////////////
    // Helpers
    ////////////////////////////////////////////////////////////////////////////////
    private static readonly Expression<Func<Room, RoomDto>> RoomDtoProjection = room =>
        new()
        {
            Id = room.Id,
            ClinicId = room.ClinicId,
            ClinicName = room.Clinic.Tenancy.OrgName,
            Title = room.Title,
            PricePerUse = room.PricePerUse,
            Description = room.Description,
            Img = room.Img
        };
}
