using HBKPlatform.Database;

namespace HBKPlatform.Services;

public interface ITimeslotService
{
    public Task CreateTimeslots(List<Timeslot> timeslots);
}