using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class AvailabilityManagementService(IUserService _userService, IAvailabilityRepository _availabilityRepo, IConfigurationService _configService) : IAvailabilityManagementService
{
    public async Task<AvailabilityManagementIndex> GetAvailabilityManagementIndexModel()
    {
        var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
        var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);

        var avIdx = new AvailabilityManagementIndex() { WeekDates = new Dictionary<int, string>() };
        avIdx.WeekDates[currentWeek] = DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate);
        for (int i = currentWeek+1; i < currentWeek + 52; i++)
        {
            avIdx.WeekDates[i] = DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, i);
        }

        return avIdx;
    }

    public async Task<AvailabilityModel> GetAvailabilityModelForWeek(int weekNum)
    {
        var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
        var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);
        var dateRangeStr = currentWeek == weekNum
            ? DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate)
            : DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, weekNum);
        
        return new AvailabilityModel()
        {
            WeekStr = dateRangeStr,
            WeekNum = weekNum
        };
    }

    public async Task UpdateAvailabilityForWeek(int weekNum, AvailabilityModel model)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
    }
    
}