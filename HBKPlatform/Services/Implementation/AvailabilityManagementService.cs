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
    
        /// <summary>
        /// Used by both Practitioner and Room availability.
        /// If RoomId is null, the model will just return a list of weekNums up to the limit.
        /// </summary>
        /// <param name="roomId">RoomId for the availability management model, if null, this is for a
        /// practitioner.</param>
        /// <returns></returns>
        public async Task<AvailabilityManagementIndex> GetAvailabilityManagementIndexModel(int? roomId)
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);

            var avIdx = new AvailabilityManagementIndex() { WeekDates = new Dictionary<int, string>(), RoomId=roomId };
            avIdx.WeekDates[currentWeek] = DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate);
            for (int i = currentWeek+1; i < currentWeek + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS; i++)
            {
                avIdx.WeekDates[i] = DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, i);
            }

            return avIdx;
        }

        ////////////////////////////////////////////////////////////////////////////////  
        // PRACTITIONER METHODS
        ////////////////////////////////////////////////////////////////////////////////  
        
        public async Task<AvailabilityModel> GetPractitionerModelForWeek(int weekNum)
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

            _currentAvailability = await _availabilityRepo.GetPractitionerLookupForWeek(pracId, weekNum);
            _indefiniteAvailability = await _availabilityRepo.GetPractitionerLookupForIndef( pracId);
        
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
        public async Task<AvailabilityModel> GetPractitionerModelForIndef()
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
        
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();

            // identical for indef model construction
            _indefiniteAvailability = await _availabilityRepo.GetPractitionerLookupForIndef(pracId);
            _currentAvailability = _indefiniteAvailability.Values.ToList();
        
            return new AvailabilityModel()
            {
                DailyTimeslotLookup = BuildAvaLiteDict(allTimeslots),
            };
        }


        public async Task UpdatePractitionerForWeek(int weekNum, UpdatedAvailability model)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.UpdatePracForWeek(weekNum, pracId, model.Updated);
        }

        public async Task RevertPractitionerForWeek(int weekNum)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.ClearPractitionerForWeek(weekNum, pracId);
        }
    
        public async Task UpdatePractitionerForIndef(UpdatedAvailability model)
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            await _availabilityRepo.UpdatePractitionerForIndef(pracId, model.Updated);
        }

        public async Task ClearPractitonerForIndef()
        {
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            await _availabilityRepo.ClearPractitionerForIndef(pracId);
        }
        
        
        ////////////////////////////////////////////////////////////////////////////////  
        // ROOM METHODS
        ////////////////////////////////////////////////////////////////////////////////  
        
        public async Task<AvailabilityModel> GetRoomModelForWeek(int roomId, int weekNum)
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
        
            var currentWeek = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            if (weekNum < currentWeek || weekNum > currentWeek + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            
            var dateRangeStr = currentWeek == weekNum
                ? DateTimeHelper.GetDateRangeStringForThisWeek(dbStartDate)
                : DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, weekNum);
        
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();

            _currentAvailability = await _availabilityRepo.GetRoomLookupForWeek(roomId, weekNum);
            _indefiniteAvailability = await _availabilityRepo.GetRoomLookupForIndef(roomId);
        
            return new AvailabilityModel()
            {
                WeekStr = dateRangeStr,
                WeekNum = weekNum,
                DailyTimeslotLookup = BuildAvaLiteDict(allTimeslots),
                RoomId = roomId
            };
        }
    
        /// <summary>
        /// Get the availability model for all weeks.
        /// </summary>
        public async Task<AvailabilityModel> GetRoomModelForIndef(int roomId)
        {
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();

            // identical for indef model construction
            _indefiniteAvailability = await _availabilityRepo.GetRoomLookupForIndef(roomId);
            _currentAvailability = _indefiniteAvailability.Values.ToList();
        
            return new AvailabilityModel()
            {
                DailyTimeslotLookup = BuildAvaLiteDict(allTimeslots),
                RoomId = roomId
            };
        }


        public async Task UpdateRoomForWeek(int roomId, int weekNum, UpdatedAvailability model)
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.UpdateRoomForWeek(weekNum, roomId, model.Updated);
        }

        public async Task ClearRoomForWeek(int roomId, int weekNum)
        {
            var dbStartDate = (await _configService.GetSettingOrDefault("DbStartDate")).Value;
            var currentWeekNum = DateTimeHelper.CurrentWeekNum(dbStartDate);
            
            // Check week number is in range
            if (weekNum < currentWeekNum || weekNum > currentWeekNum + DefaultSettings.AVAILABILITY_ADVANCE_WEEKS)
            {
                throw new InvalidUserOperationException("Week number out of permitted range.");
            }
            await _availabilityRepo.ClearRoomForWeek(weekNum, roomId);
        }
    
        public async Task UpdateRoomForIndef(int roomId, UpdatedAvailability model)
        {
            await _availabilityRepo.UpdateRoomForIndef(roomId, model.Updated);
        }

        public async Task ClearRoomForIndef(int roomId)
        {
            await _availabilityRepo.ClearRoomForIndef(roomId);
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // HELPERS
        //////////////////////////////////////////////////////////////////////////////// 
        
        /// <summary>
        /// Build availability dictionary from timeslots. Used both per-week and for all weeks. Used in the Availability
        /// Management View.
        /// </summary>
        private Dictionary<Enums.Day, List<AvailabilityLite>> BuildAvaLiteDict(List<TimeslotDto> allTimeslots)
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
                    Description = x.Time.ToShortTimeString(), IsAvailable = IsAvailable(x.TimeslotIdx), TimeslotId = x.TimeslotIdx, IsIndefiniteAvailable = IsIndefiniteAvailable(x.TimeslotIdx)
                }).ToList();
            }

            return dailyTimeslotLookup;
        }

        /// <summary>
        ///  Determine whether the db TimeslotAvailability enum resolves to true or false.
        ///  A TS is unavailable if explicitly set as unavailable or there is no entry in the DB (HBK-36)
        /// Previously, no data was assumed as available
        /// </summary>
        private bool IsAvailable(int tsId)
        {
            var avaDto = _currentAvailability.FirstOrDefault(x => x.TimeslotId == tsId);
            if (avaDto == null)
            {
                return false;
            }
        
            switch (avaDto.Availability)
            {
                case Enums.TimeslotAvailability.Available: return true;
                case Enums.TimeslotAvailability.Unavailable: default: return false;
            }
        }
    
        /// <summary>
        /// Return TRUE if there is a matching indefinite availability for the timeslotId, and it is available
        /// </summary>
        private bool IsIndefiniteAvailable(int tsId)
        {
            if (!_indefiniteAvailability.TryGetValue(tsId, out var avaDto))
            {
                return false;
            }

            return avaDto.IsIndefinite && avaDto.Availability == Enums.TimeslotAvailability.Available;
        }
    
    }
}
