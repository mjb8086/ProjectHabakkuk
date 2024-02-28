using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;
using Moq;

namespace HBK.Test
{
    /// <summary>
    /// HBKPlatform Booking Service unit tests.
    /// 
    /// Author: Mark Brown
    /// Authored: 19/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class BookingServiceUnitTests
    {
        public const int DEFAULT_DURATION = 30;
        public const int CLINIC_ID = 1;
        public const string DB_START_DATE = "2024-01-01";
    
        private List<TimeslotDto> GenerateTimeslots() 
        {
            var timeslots = new List<TimeslotDto>();
            var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
            foreach (var day in days)
            {
                var time = new TimeOnly(12, 00, 00);
                var maxTime = new TimeOnly( 18, 00, 00);
            
                while (time <= maxTime)
                {
                    timeslots.Add(new TimeslotDto() { Day = day, Time = time, Duration = DEFAULT_DURATION});
                    time = time.AddMinutes(DEFAULT_DURATION);
                }
            }
            return timeslots;
        }

        [Fact]
        public async Task GetTimeslotsForBookingTest_1()
        {
            var timeslotList = GenerateTimeslots();
            var mockTimeslotRepo = new Mock<ITimeslotRepository>();
            var mockConfigService = new Mock<IConfigurationService>();
            var mockDateTimeHelper = new Mock<IDateTimeWrapper>();
            mockTimeslotRepo.Setup(x => x.GetClinicTimeslots()).ReturnsAsync(timeslotList);
            mockConfigService.Setup(x => x.GetSettingOrDefault("DbStartDate")).ReturnsAsync(new SettingDto() {Value = DB_START_DATE});
            mockConfigService.Setup(x => x.GetSettingOrDefault("BookingAdvanceWeeks")).ReturnsAsync(new SettingDto() {Value = "2"});
            mockDateTimeHelper.Setup(x => x.Now).Returns(new DateTime(2024, 01, 17, 14, 00, 00));

            var bookingService = new BookingService(mockTimeslotRepo.Object, null, null, null, mockConfigService.Object, mockDateTimeHelper.Object, null);
            // todo: mock other dependencies
//        var timeslots = bookingService.Get
        }



    }
}