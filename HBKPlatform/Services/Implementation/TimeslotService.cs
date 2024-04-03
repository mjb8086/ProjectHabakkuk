using HBKPlatform.Database;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation
{
    public class TimeslotService(ITimeslotRepository _tsRepo, ITimeslotRepository _timeslotRepo, IConfigurationService _config, IDateTimeWrapper _dateTime): ITimeslotService
    {
        public async Task CreateTimeslots(List<Timeslot> timeslots)
        {
            await _tsRepo.Create(timeslots);
        }
        
        /// <summary>
        /// Get a list of timeslots in the future, from this week day until the upper bound of the BookingAdvanceWeeks
        /// value. Each Ts DTO will have its weekNum field populated.
        /// Timeslots returned from this method do not exclude unavailable - we don't know the conditions just yet!
        /// </summary>
        public async Task<SortedSet<TimeslotDto>> GetPopulatedFutureTimeslots()
        {
            var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var bookingAdvance = int.Parse((await _config.GetSettingOrDefault("BookingAdvanceWeeks")).Value);
            var now = _dateTime.Now;
            
            return TimeslotHelper.GetPopulatedFutureTimeslots(now, allTimeslots, dbStartDate, bookingAdvance);
        }
    }
}