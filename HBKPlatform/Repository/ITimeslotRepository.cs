using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface ITimeslotRepository
{
    public Task<TimeslotDto> GetTimeslot(int timeslotId);
    public Task<List<TimeslotDto>> GetClinicTimeslots(int clinicId);
    public Task Create(TimeslotDto timeslotDto);
}