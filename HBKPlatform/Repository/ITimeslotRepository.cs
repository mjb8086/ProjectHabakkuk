using HBKPlatform.Database;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface ITimeslotRepository
{
    public Task<TimeslotDto> GetTimeslot(int timeslotId);
    public Task<List<TimeslotDto>> GetClinicTimeslots();
    public Task Create(TimeslotDto timeslotDto);
    public Task Create(List<Timeslot> timeslots);
}