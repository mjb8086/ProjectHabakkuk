using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class AvailabilityManagementService(IUserService _userService, IAvailabilityRepository _availabilityRepo, 
    IConfigurationService _configService, ITimeslotRepository _timeslotRepo) : IAvailabilityManagementService
{
    private Dictionary<int, Enums.TimeslotAvailability> CurrentAvailability;
    
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
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        
        var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);
        var dateRangeStr = currentWeek == weekNum
            ? DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate)
            : DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, weekNum);
        
        var allTimeslots = await _timeslotRepo.GetClinicTimeslots(clinicId);

        CurrentAvailability = await _availabilityRepo.GetAvailabilityLookupForWeek(clinicId, pracId, weekNum);
        
        return new AvailabilityModel()
        {
            WeekStr = dateRangeStr,
            WeekNum = weekNum,
            DailyTimeslotLookup = BuildAvaLiteDict(allTimeslots)
        };
    }
    
    /// <summary>
    /// Get the availability model for all weeks.
    /// </summary>
    public async Task<AvailabilityModel> GetAvailabilityModelForIndef()
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        
        var allTimeslots = await _timeslotRepo.GetClinicTimeslots(clinicId);

        CurrentAvailability = await _availabilityRepo.GetAvailabilityLookupForIndef(clinicId, pracId);
        
        return new AvailabilityModel()
        {
            DailyTimeslotLookup = BuildAvaLiteDict(allTimeslots),
        };
    }

    /// <summary>
    /// Build availability dictionary from timeslots. Used both per-week and for all weeks.
    /// </summary>
    public Dictionary<Enums.Day, List<AvailabilityLite>> BuildAvaLiteDict(List<TimeslotDto> allTimeslots)
    {
        if (CurrentAvailability == null) throw new NullReferenceException("Current availability is not populated.");
        var dailyTimeslotLookup = new Dictionary<Enums.Day, List<AvailabilityLite>>();
        foreach (var day in new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday})
        {
            var thisDayTs = allTimeslots.Where(x => x.Day == day).OrderBy(x => x.Time).ToList();
            dailyTimeslotLookup[day] = thisDayTs.Select(x => new AvailabilityLite()
            {
                // IsAvailable depends on CurrentAvailability being populated.
                Description = x.Time.ToShortTimeString(), IsAvailable = IsAvailable(x.Id), TimeslotId = x.Id
            }).ToList();
        }

        return dailyTimeslotLookup;
    }

    /// <summary>
    ///  Determine whether the db TimeslotAvailability enum is true or false.
    /// If no value is set, we assume true.
    /// </summary>
    private bool IsAvailable(int tsId)
    {
        if (!CurrentAvailability.TryGetValue(tsId, out var avaEnum))
        {
            return true;
        }
        
        switch (avaEnum)
        {
            case Enums.TimeslotAvailability.Available: return true;
            case Enums.TimeslotAvailability.Unavailable: return false;
            default: return true;
        }
    }

    public async Task UpdateAvailabilityForWeek(int weekNum, UpdatedAvailability model)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        await _availabilityRepo.UpdateAvailabilityForWeek(weekNum, pracId, clinicId, model.Updated);
    }

    public async Task RevertAvailabilityForWeek(int weekNum)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        await _availabilityRepo.RevertAvailabilityForWeek(weekNum, pracId, clinicId);
    }
    
    public async Task UpdateAvailabilityForIndef(UpdatedAvailability model)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        await _availabilityRepo.UpdateAvailabilityForIndef(pracId, clinicId, model.Updated);
    }

    public async Task RevertAvailabilityForIndef()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        await _availabilityRepo.RevertAvailabilityForIndef(pracId, clinicId);
    }
    
}