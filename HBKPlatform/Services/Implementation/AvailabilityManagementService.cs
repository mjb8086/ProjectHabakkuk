using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;
using MissingMemberException = System.MissingMemberException;

namespace HBKPlatform.Services.Implementation
{
    public class AvailabilityManagementService(IUserService _userService, IAvailabilityRepository _availabilityRepo, 
        IConfigurationService _configService, ITimeslotRepository _timeslotRepo) : IAvailabilityManagementService
    {
        private List<TimeslotAvailabilityDto> _currentAvailability;
        private Dictionary<int, TimeslotAvailabilityDto> _indefiniteAvailability;
    
        public async Task<AvailabilityManagementIndex> GetAvailabilityManagementIndexModel()
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);

            var avIdx = new AvailabilityManagementIndex() { WeekDates = new Dictionary<int, string>() };
            avIdx.WeekDates[currentWeek] = DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate);
            for (int i = currentWeek+1; i < currentWeek + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS; i++)
            {
                avIdx.WeekDates[i] = DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, i);
            }

            return avIdx;
        }

        public async Task<AvailabilityModel> GetAvailabilityModelForWeek(int weekNum)
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
        
            var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            if (weekNum < currentWeek || weekNum > currentWeek + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            
            var dateRangeStr = currentWeek == weekNum
                ? DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate)
                : DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, weekNum);
        
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();

            _currentAvailability = await _availabilityRepo.GetAvailabilityLookupForWeek(pracId, weekNum);
            _indefiniteAvailability = await _availabilityRepo.GetAvailabilityLookupForIndef( pracId);
        
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
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
        
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();

            // identical for indef model construction
            _indefiniteAvailability = await _availabilityRepo.GetAvailabilityLookupForIndef(pracId);
            _currentAvailability = _indefiniteAvailability.Values.ToList();
        
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
            if (_currentAvailability == null) throw new MissingMemberException("Current availability is not populated.");
            if (_indefiniteAvailability == null) throw new MissingMemberException("Indefinite availability is not populated.");
            var dailyTimeslotLookup = new Dictionary<Enums.Day, List<AvailabilityLite>>();
            foreach (var day in new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday})
            {
                var thisDayTs = allTimeslots.Where(x => x.Day == day).OrderBy(x => x.Time).ToList();
                dailyTimeslotLookup[day] = thisDayTs.Select(x => new AvailabilityLite()
                {
                    // IsAvailable depends on CurrentAvailability being populated.
                    Description = x.Time.ToShortTimeString(), IsAvailable = IsAvailable(x.Id), TimeslotId = x.Id, IsIndefinite = IsIndefiniteUnavailable(x.Id)
                }).ToList();
            }

            return dailyTimeslotLookup;
        }

        /// <summary>
        ///  Determine whether the db TimeslotAvailability enum resolves to true or false.
        /// If no value is set, we assume true.
        /// </summary>
        private bool IsAvailable(int tsId)
        {
            var avaDto = _currentAvailability.FirstOrDefault(x => x.TimeslotId == tsId);
            if (avaDto == null)
            {
                return true;
            }
        
            switch (avaDto.Availability)
            {
                case Enums.TimeslotAvailability.Available: return true;
                case Enums.TimeslotAvailability.Unavailable: return false;
                default: return true;
            }
        }
    
        /// <summary>
        /// Return TRUE if there is a matching indefinite availability for the timeslotId, and it is unavailable
        /// </summary>
        private bool IsIndefiniteUnavailable(int tsId)
        {
            if (!_indefiniteAvailability.TryGetValue(tsId, out var avaDto))
            {
                return false;
            }

            return avaDto.IsIndefinite && avaDto.Availability == Enums.TimeslotAvailability.Unavailable;

        }

        public async Task UpdateAvailabilityForWeek(int weekNum, UpdatedAvailability model)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.UpdateAvailabilityForWeek(weekNum, pracId, model.Updated);
        }

        public async Task RevertAvailabilityForWeek(int weekNum)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.RevertAvailabilityForWeek(weekNum, pracId);
        }
    
        public async Task UpdateAvailabilityForIndef(UpdatedAvailability model)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            await _availabilityRepo.UpdateAvailabilityForIndef(pracId, model.Updated);
        }

        public async Task RevertAvailabilityForIndef()
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            await _availabilityRepo.RevertAvailabilityForIndef(pracId);
        }
    
    }
}
