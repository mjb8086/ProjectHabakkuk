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
        // Static data
        const int DEFAULT_DURATION = 60;
        const string DB_START_DATE = "2024-01-01";
        const int FAKE_TREATMENT_ID = 1;
        const int FAKE_CLINIC_ID = 2;
        const int FAKE_PRAC_ID = 3;
        
        readonly Dictionary<int, TreatmentDto> FAKE_TREATMENTS = new ()
        {
            {FAKE_TREATMENT_ID, new TreatmentDto() { Id = FAKE_TREATMENT_ID, Title = "Arse Shaving", Description = "Painful, but try it once, you'll love it forever.", Requestability = Enums.TreatmentRequestability.ClientAndPrac}},
            {5, new TreatmentDto() {Id = 5, Title = "Wrong Trouser Removal", Description = "Discount for vertically-challenged penguins.", Requestability = Enums.TreatmentRequestability.PracOnly}},
            {6, new TreatmentDto() {Id = 6, Title = "Invisibility cure", Description = "paint and glitterbomb.", Requestability = Enums.TreatmentRequestability.None}},
        };
    
        private List<TimeslotDto> GenerateTimeslots(int startHour, int endHour, int duration, int weekNum = 0) 
        {
            var timeslots = new List<TimeslotDto>();
            var days = new [] {Enums.Day.Monday, Enums.Day.Tuesday, Enums.Day.Wednesday, Enums.Day.Thursday, Enums.Day.Friday, Enums.Day.Saturday, Enums.Day.Sunday};
            int id = 0;
            
            foreach (var day in days)
            {
                var time = new TimeOnly(startHour, 00, 00);
                var maxTime = new TimeOnly( endHour, 00, 00);
            
                while (time <= maxTime)
                {
                    var newTs = new TimeslotDto()
                    {
                        Id = id, Day = day, Time = time, Duration = duration, WeekNum = weekNum, 
                    };
                    if(weekNum > 0) newTs.Description = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(DB_START_DATE, newTs));
                    timeslots.Add(newTs);
                    
                    time = time.AddMinutes(duration);
                    id++;
                }
            }
            return timeslots;
        }
        

        /// <summary>
        /// check that the timeslots returned are expected - first, all in the next two weeks when there are no
        /// appointments, then some other cases.
        /// Nice and even because it assumes today is the first hour of the first day in the set of timeslots
        /// </summary>
        [Theory]
        [InlineData(new [] {3,4}, "2", "2024-01-15 12:00")]
        [InlineData(new [] {3,4,5,6}, "4", "2024-01-15 12:00")]
        [InlineData(new [] {1}, "1", "2024-01-01 12:00")]
        public async Task GetTimeslotsClientView_ReturnsExpectedTimeslots(int[] weekNums, string advanceWeeks, string dtString)
        {
            var now = DateTime.Parse(dtString);
            // Arrange
            var timeslotList = GenerateTimeslots(12, 15, DEFAULT_DURATION);
            
            var mockTimeslotRepo = new Mock<ITimeslotRepository>();
            var mockConfigService = new Mock<IConfigurationService>();
            var mockDateTimeHelper = new Mock<IDateTimeWrapper>();
            var mockUserService = new Mock<IUserService>();
            var mockCacheService = new Mock<ICacheService>();
            var mockAvaRepo = new Mock<IAvailabilityRepository>();
            var mockApptRepo = new Mock<IAppointmentRepository>();
            
            mockTimeslotRepo.Setup(x => x.GetClinicTimeslots()).ReturnsAsync(timeslotList);
            mockConfigService.Setup(x => x.GetSettingOrDefault("DbStartDate")).ReturnsAsync(new SettingDto() {Value = DB_START_DATE});
            mockConfigService.Setup(x => x.GetSettingOrDefault("BookingAdvanceWeeks")).ReturnsAsync(new SettingDto() {Value = advanceWeeks});
            mockDateTimeHelper.Setup(x => x.Now).Returns(now);
            mockUserService.Setup(x => x.GetClaimFromCookie("ClinicId")).Returns(FAKE_CLINIC_ID);
            mockCacheService.Setup(x => x.GetLeadPracId(FAKE_CLINIC_ID)).Returns(FAKE_PRAC_ID);
            mockCacheService.Setup(x => x.GetTreatments()).ReturnsAsync(FAKE_TREATMENTS);
            mockAvaRepo.Setup(x => x.GetAvailabilityLookupForWeeks(FAKE_PRAC_ID, weekNums))
                .ReturnsAsync(new List<TimeslotAvailabilityDto>()); 
            // Return empty availability for this test - a missing availability value for any week is reckoned as available
            mockAvaRepo.Setup(x => x.GetAvailabilityLookupForIndef(FAKE_PRAC_ID))
                .ReturnsAsync(new Dictionary<int, TimeslotAvailabilityDto>());
            mockApptRepo.Setup(x => x.GetFutureAppointmentsForPractitioner(FAKE_PRAC_ID, mockDateTimeHelper.Object.Now))
                .ReturnsAsync(new List<AppointmentDto>());

            // Instantiate booking service
            var bookingService = new BookingService(mockTimeslotRepo.Object, mockUserService.Object, mockCacheService.Object, mockApptRepo.Object, mockConfigService.Object, mockDateTimeHelper.Object, mockAvaRepo.Object);
            
            // Act
            var timeslots = (await bookingService.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            
            // Assert
            var expected = new List<TimeslotDto>();
            foreach (var weekNum in weekNums)
            {
                expected.AddRange(GenerateTimeslots(12, 15, DEFAULT_DURATION, weekNum));
            }
            
            Assert.True(timeslots.CompareTsList(expected));
        }
        
        [Theory]
        [InlineData(new [] {3,4}, "2", "2024-01-15 12:00", "2024-01-15 12:00", "2024-01-15 12:00", 322)]
        public async Task GetTimeslotsClientView_BoundsCheck(int[] weekNums, string advanceWeeks, string dtString, string firstTs, string lastTs, int expectedTsCount)
        {
            var now = DateTime.Parse(dtString);
            // Arrange
            var timeslotList = GenerateTimeslots(09, 18, 30);
            
            var mockTimeslotRepo = new Mock<ITimeslotRepository>();
            var mockConfigService = new Mock<IConfigurationService>();
            var mockDateTimeHelper = new Mock<IDateTimeWrapper>();
            var mockUserService = new Mock<IUserService>();
            var mockCacheService = new Mock<ICacheService>();
            var mockAvaRepo = new Mock<IAvailabilityRepository>();
            var mockApptRepo = new Mock<IAppointmentRepository>();
            
            mockTimeslotRepo.Setup(x => x.GetClinicTimeslots()).ReturnsAsync(timeslotList);
            mockConfigService.Setup(x => x.GetSettingOrDefault("DbStartDate")).ReturnsAsync(new SettingDto() {Value = DB_START_DATE});
            mockConfigService.Setup(x => x.GetSettingOrDefault("BookingAdvanceWeeks")).ReturnsAsync(new SettingDto() {Value = advanceWeeks});
            mockDateTimeHelper.Setup(x => x.Now).Returns(now);
            mockUserService.Setup(x => x.GetClaimFromCookie("ClinicId")).Returns(FAKE_CLINIC_ID);
            mockCacheService.Setup(x => x.GetLeadPracId(FAKE_CLINIC_ID)).Returns(FAKE_PRAC_ID);
            mockCacheService.Setup(x => x.GetTreatments()).ReturnsAsync(FAKE_TREATMENTS);
            mockAvaRepo.Setup(x => x.GetAvailabilityLookupForWeeks(FAKE_PRAC_ID, weekNums))
                .ReturnsAsync(new List<TimeslotAvailabilityDto>()); 
            // Return empty availability for this test - a missing availability value for any week is reckoned as available
            mockAvaRepo.Setup(x => x.GetAvailabilityLookupForIndef(FAKE_PRAC_ID))
                .ReturnsAsync(new Dictionary<int, TimeslotAvailabilityDto>());
            mockApptRepo.Setup(x => x.GetFutureAppointmentsForPractitioner(FAKE_PRAC_ID, mockDateTimeHelper.Object.Now))
                .ReturnsAsync(new List<AppointmentDto>());

            // Instantiate booking service
            var bookingService = new BookingService(mockTimeslotRepo.Object, mockUserService.Object, mockCacheService.Object, mockApptRepo.Object, mockConfigService.Object, mockDateTimeHelper.Object, mockAvaRepo.Object);
            
            // Act
            var timeslots = (await bookingService.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            
            // Assert
            var actualTsCount = timeslots.Count; 
            Assert.Equal(expectedTsCount, actualTsCount);
            
            // Create timeslots from InlineData.
        }
        
        // then with some appointments already booked - ensure those slots are not available
        
        // try to book an appointment, ensure the exception prevents this

    }
}