using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;
using HBKPlatform.Services;
using HBKPlatform.Services.Implementation;
using Microsoft.Extensions.Logging;
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
        //////////////////////////////////////////////////////////////////////////////// 
        // Static data
        //////////////////////////////////////////////////////////////////////////////// 
        const int DEFAULT_DURATION = 60;
        const string DB_START_DATE = "2024-01-01";
        const int FAKE_TREATMENT_ID = 1;
        const int FAKE_PRACTICE_ID = 2;
        const int FAKE_PRACTITIONER_ID = 3;
        
        readonly Dictionary<int, TreatmentDto> FAKE_TREATMENTS = new ()
        {
            {FAKE_TREATMENT_ID, new TreatmentDto() { Id = FAKE_TREATMENT_ID, Title = "Arse Shaving", Description = "Painful, but try it once, you'll love it forever.", Requestability = Enums.TreatmentRequestability.ClientAndPrac}},
            {5, new TreatmentDto() {Id = 5, Title = "Wrong Trouser Removal", Description = "Discount for vertically-challenged penguins.", Requestability = Enums.TreatmentRequestability.PracOnly}},
            {6, new TreatmentDto() {Id = 6, Title = "Invisibility cure", Description = "paint and glitterbomb.", Requestability = Enums.TreatmentRequestability.None}},
        };
        
        //////////////////////////////////////////////////////////////////////////////// 
        // Helpers
        //////////////////////////////////////////////////////////////////////////////// 
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
                        Id = id++, Day = day, Time = time, Duration = duration, WeekNum = weekNum, 
                    };
                    if(weekNum > 0) newTs.Description = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(DB_START_DATE, newTs));
                    timeslots.Add(newTs);
                    time = time.AddMinutes(duration);
                }
            }
            return timeslots;
        }

        // Save the repetition...
        public BookingService GetMockedBookingService(List<TimeslotDto> timeslotList, string advanceWeeks, DateTime now, 
            List<AppointmentDto> appointments, Dictionary<int, TimeslotAvailabilityDto> indefAvailability, int[]? weekNums = null)
        {
            var mockTimeslotRepo = new Mock<ITimeslotRepository>();
            var mockConfigService = new Mock<IConfigurationService>();
            var mockDateTimeHelper = new Mock<IDateTimeWrapper>();
            var mockUserService = new Mock<IUserService>();
            var mockCacheService = new Mock<ICacheService>();
            var mockAvaRepo = new Mock<IAvailabilityRepository>();
            var mockApptRepo = new Mock<IAppointmentRepository>();
            
            mockTimeslotRepo.Setup(x => x.GetPracticeTimeslots()).ReturnsAsync(timeslotList);
            mockConfigService.Setup(x => x.GetSettingOrDefault("DbStartDate")).ReturnsAsync(new SettingDto() {Value = DB_START_DATE});
            mockConfigService.Setup(x => x.GetSettingOrDefault("BookingAdvanceWeeks")).ReturnsAsync(new SettingDto() {Value = advanceWeeks});
            mockDateTimeHelper.Setup(x => x.Now).Returns(now);
            mockUserService.Setup(x => x.GetClaimFromCookie("PracticeId")).Returns(FAKE_PRACTICE_ID);
            mockCacheService.Setup(x => x.GetLeadPractitionerId(FAKE_PRACTICE_ID)).Returns(FAKE_PRACTITIONER_ID);
            mockCacheService.Setup(x => x.GetTreatments()).ReturnsAsync(FAKE_TREATMENTS);
            if (weekNums != null)
            {
                mockAvaRepo.Setup(x => x.GetPractitionerLookupForWeeks(FAKE_PRACTITIONER_ID, weekNums))
                    .ReturnsAsync(new List<TimeslotAvailabilityDto>());
            }
            else
            {
                mockAvaRepo.Setup(x => x.GetPractitionerLookupForWeeks(FAKE_PRACTITIONER_ID, It.IsAny<int[]>()))
                    .ReturnsAsync(new List<TimeslotAvailabilityDto>());
            }
            // Return empty availability when not specified - a missing availability value for any week is reckoned as available
            mockAvaRepo.Setup(x => x.GetPractitionerLookupForIndef(FAKE_PRACTITIONER_ID))
                .ReturnsAsync(indefAvailability);
            mockApptRepo.Setup(x => x.GetFutureAppointmentsForPractitioner(FAKE_PRACTITIONER_ID, mockDateTimeHelper.Object.Now))
                .ReturnsAsync(appointments);

            return new BookingService(mockTimeslotRepo.Object, mockUserService.Object, mockCacheService.Object, 
                mockApptRepo.Object, mockConfigService.Object, mockDateTimeHelper.Object, mockAvaRepo.Object, 
                new Mock<IRoomReservationService>().Object, new Mock<ILogger<BookingService>>().Object);
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // Begin tests
        //////////////////////////////////////////////////////////////////////////////// 

        /// <summary>
        /// check that the timeslots returned are expected - first, all in the next two weeks when there are no
        /// appointments, then some other cases.
        /// This simulates the choose a timeslot booking screen - the timeslots have the weeknum populated
        /// Nice and even because it assumes today is the first hour of the first day in the set of timeslots
        /// </summary>
        [Theory]
        [InlineData(new [] {3,4}, "2", "2024-01-15 12:00")]
        [InlineData(new [] {3,4,5,6}, "4", "2024-01-15 12:00")]
        [InlineData(new [] {1}, "1", "2024-01-01 12:00")]
        public async Task GetTimeslotsClientView_ReturnsExpectedTimeslots(int[] weekNums, string advanceWeeks, string currentDt)
        {
            var now = DateTime.Parse(currentDt);
            // Arrange
            var timeslotList = GenerateTimeslots(12, 15, DEFAULT_DURATION);
            
            // Instantiate booking service
            var bookingService = GetMockedBookingService(timeslotList, advanceWeeks, now, new List<AppointmentDto>(), new Dictionary<int, TimeslotAvailabilityDto>(), weekNums);
            
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
        
        /// <summary>
        /// Verify availability timeslots fall within expected weekNums - based on the current dt
        /// All timeslots generated start ar 09:00 and end at 18:00 for these tests
        /// This simulates the choose a timeslot booking screen - the timeslots have their weeknum populated
        /// </summary>
        [Theory]
        [InlineData(new [] {3,4}, "2", "2024-01-15 09:00", "2024-01-15 09:00", "2024-01-28 18:00", 266)]
        [InlineData(new [] {3,4,5}, "2", "2024-01-17 11:20", "2024-01-17 11:30", "2024-01-31 11:00", 266)]
        [InlineData(new [] {3,4,5}, "2", "2024-01-17 07:20", "2024-01-17 09:00", "2024-01-30 18:00", 266)] // what happens outside of hours?
        [InlineData(new [] {3,4,5}, "2", "2024-01-17 21:12", "2024-01-18 09:00", "2024-01-31 18:00", 266)] // what happens outside of hours?
        [InlineData(new [] {3,4,5}, "2", "2024-01-17 08:59:59", "2024-01-17 09:00", "2024-01-30 18:00", 266)] // on the dot just before it
        [InlineData(new [] {3,4,5}, "2", "2024-01-17 17:59:59", "2024-01-17 18:00", "2024-01-31 17:30:00", 266)] // and just at the end
        [InlineData(new [] {6}, "1", "2024-02-05 09:00", "2024-02-05 09:00", "2024-02-11 18:00", 133)] // one advance week
        [InlineData(new [] {6, 7}, "1", "2024-02-07 11:42", "2024-02-07 12:00", "2024-02-14 11:30", 133)] // one advance week
        public async Task GetTimeslotsClientView_BoundsCheck(int[] weekNums, string advanceWeeks, string currentDt, string expectedFirstTs, string expectedLastTs, int expectedTsCount)
        {
            var now = DateTime.Parse(currentDt);
            // Arrange
            var timeslotList = GenerateTimeslots(09, 18, 30);
            
            // Instantiate booking service
            var bookingService = GetMockedBookingService(timeslotList, advanceWeeks, now, new List<AppointmentDto>(), new Dictionary<int, TimeslotAvailabilityDto>(), null);
            
            // Act
            var timeslots = (await bookingService.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            
            // Assert
            // number of Timeslots as expected?
            var actualTsCount = timeslots.Count; 
            Assert.Equal(expectedTsCount, actualTsCount);

            // Weeknums as expected?
            var actualWeekNums = timeslots.Select(x => x.WeekNum).Distinct().ToArray();
            Assert.Equal(weekNums, actualWeekNums);
            
            // First and last timeslots as expected?
            Assert.Equal(DateTime.Parse(expectedFirstTs), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslots.First()));
            Assert.Equal(DateTime.Parse(expectedLastTs), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslots[^1]));
        }
        
        /// <summary>
        /// test with some appointments already booked - ensure those slots are not available
        ///</summary>
        [Fact]
        public async Task TestBookingView_ExcludesBookedTimeslots()
        {
            var WEEK_NUM = 5;
            var TS_ID = 20;
            
            // Arrange
            var timeslotList = GenerateTimeslots(09, 18, 30, WEEK_NUM);
            // NB: AppointmentRepo includes Timeslot data in select, thus the appointment's timeslot must be instantiated
            var appointments = new List<AppointmentDto>()
            {
                new () { PractitionerId = FAKE_PRACTITIONER_ID, WeekNum = WEEK_NUM, Timeslot = timeslotList.First(x => x.Id == TS_ID), Status = Enums.AppointmentStatus.Live, TreatmentId = FAKE_TREATMENT_ID}
            };
            
            // Instantiate booking service
            // weekNum is 5 on the 2nd of Feb 
            var bookingService = GetMockedBookingService(timeslotList, "2", new DateTime(2024,01,29, 15,30, 00) , appointments, new Dictionary<int, TimeslotAvailabilityDto>());
            var bookingServiceNoAppts = GetMockedBookingService(timeslotList, "2", new DateTime(2024,01,29, 15,30, 00) , new List<AppointmentDto>(), new Dictionary<int, TimeslotAvailabilityDto>());
            
            // Act
            var timeslots = (await bookingService.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            var timeslotsNoAppts = (await bookingServiceNoAppts.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            
            // Assert
            // number of available Timeslots as expected?
            Assert.Equal(265, timeslots.Count);
            Assert.Equal(266, timeslotsNoAppts.Count);
            
            // Is the booked timeslot excluded? 
            Assert.False(timeslots.Any(x => x.Id == TS_ID && x.WeekNum == WEEK_NUM), "A booked timeslot was present in the list of available.");
            // And the control group...
            Assert.True(timeslotsNoAppts.Any(x => x.Id == TS_ID && x.WeekNum == WEEK_NUM), "An unbooked timeslot was missing from the list of available.");
        }
        
        [Fact]
        public async Task TestBookingView_ExcludesBookedTimeslots_2()
        {
            var WEEK_NUM = 5;
            
            // Arrange
            var timeslotList = GenerateTimeslots(09, 18, 30, WEEK_NUM);
            // NB: AppointmentRepo includes Timeslot data in select, thus the appointment's timeslot must be instantiated
            var appointments = new List<AppointmentDto>()
            {
                new () { PractitionerId = FAKE_PRACTITIONER_ID, WeekNum = WEEK_NUM, Timeslot = timeslotList.First(x => x.Id == 21), Status = Enums.AppointmentStatus.Live, TreatmentId = FAKE_TREATMENT_ID},
                new () { PractitionerId = FAKE_PRACTITIONER_ID, WeekNum = WEEK_NUM, Timeslot = timeslotList.First(x => x.Id == 41), Status = Enums.AppointmentStatus.Live, TreatmentId = FAKE_TREATMENT_ID},
                new () { PractitionerId = FAKE_PRACTITIONER_ID, WeekNum = WEEK_NUM, Timeslot = timeslotList.First(x => x.Id == 30), Status = Enums.AppointmentStatus.Live, TreatmentId = FAKE_TREATMENT_ID}
            };
            
            // Instantiate booking service
            // weekNum is 5 on the 2nd of Feb 
            var bookingService = GetMockedBookingService(timeslotList, "2", new DateTime(2024,01,29, 15,30, 00) , appointments, new Dictionary<int, TimeslotAvailabilityDto>());
            
            // Act
            var timeslots = (await bookingService.GetAvailableTimeslotsClientView(FAKE_TREATMENT_ID)).AvailableTimeslots;
            
            // Assert
            // number of available Timeslots as expected?
            Assert.Equal(263, timeslots.Count);
            
            // Is the booked timeslot excluded? 
            Assert.False(timeslots.Any(x => x.Id == 21 && x.WeekNum == WEEK_NUM), "A booked timeslot was present in the list of available.");
            Assert.False(timeslots.Any(x => x.Id == 30 && x.WeekNum == WEEK_NUM), "A booked timeslot was present in the list of available.");
            Assert.False(timeslots.Any(x => x.Id == 41 && x.WeekNum == WEEK_NUM), "A booked timeslot was present in the list of available.");
            
            Assert.True(timeslots.Any(x => x.Id == 22 && x.WeekNum == WEEK_NUM), "An unbooked timeslot was missing from the list of available.");
            Assert.True(timeslots.Any(x => x.Id == 23 && x.WeekNum == WEEK_NUM), "An unbooked timeslot was missing from the list of available.");
            Assert.True(timeslots.Any(x => x.Id == 29 && x.WeekNum == WEEK_NUM), "An unbooked timeslot was missing from the list of available.");
            Assert.True(timeslots.Any(x => x.Id == 31 && x.WeekNum == WEEK_NUM), "An unbooked timeslot was missing from the list of available.");
        }
        
        // TODO: More of the appointment booking tests.
        
        // ensure that cancelled appointments release their timeslot
        
        // try to double book an appointment, ensure the exception prevents this
        
        // Ensure client cannot book prac only appointments
        
        // Ensure prac can book all appt types
        
        // Ensure that global availability is applied
        
        // Ensure that per-week availability applies and only to the specified week

    }
}