using HBKPlatform.Models;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services;

public interface IAvailabilityManagementService
{
    public Task<AvailabilityManagementIndex> GetAvailabilityManagementIndexModel();

    public Task UpdateAvailabilityForWeek(int weekNum, UpdatedAvailability model);
    public Task<AvailabilityModel> GetAvailabilityModelForWeek(int weekNum);
    public Task RevertAvailabilityForWeek(int weekNum);
}