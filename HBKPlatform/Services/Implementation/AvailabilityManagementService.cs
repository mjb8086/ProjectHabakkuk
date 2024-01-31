using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class AvailabilityManagementService(IUserService _userService, IAvailabilityRepository _availabilityRepo, 
    IConfigurationService _configService, ITimeslotRepository _timeslotRepo) : IAvailabilityManagementService
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
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
        
        var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);
        var dateRangeStr = currentWeek == weekNum
            ? DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate)
            : DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, weekNum);
        
        var allTimeslots = await _timeslotRepo.GetClinicTimeslots(clinicId);

        var dailyTimeslotLookup = new Dictionary<Enums.Day, List<AvailabilityLite>>();
        foreach (var day in new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday})
        {
            var thisDayTs = allTimeslots.Where(x => x.Day == day).OrderBy(x => x.Time).ToList();
            dailyTimeslotLookup[day] = thisDayTs.Select(x => new AvailabilityLite()
            {
                Description = x.Time.ToShortTimeString(), IsAvailable = true, TimeslotId = x.Id
            }).ToList();
        }
        
        return new AvailabilityModel()
        {
            WeekStr = dateRangeStr,
            WeekNum = weekNum,
            DailyTimeslotLookup = dailyTimeslotLookup
        };
    }

    public async Task UpdateAvailabilityForWeek(int weekNum, AvailabilityModel model)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
    }
    
}