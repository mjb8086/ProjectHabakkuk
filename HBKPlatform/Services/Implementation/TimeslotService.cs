using HBKPlatform.Database;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class TimeslotService(ITimeslotRepository _tsRepo): ITimeslotService
{
    public async Task CreateTimeslots(List<Timeslot> timeslots)
    {
        await _tsRepo.Create(timeslots);
    }
}