using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using Xunit.Abstractions;

namespace HBK.Test
{
    /// <summary>
    /// HBKPlatform Date Time helper unit tests.
    /// 
    /// Author: Mark Brown
    /// Authored: 16/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class DateTimeHelperTests(ITestOutputHelper _testOutputHelper)
    {
        public static string DB_START_DATE = "2024-01-01";

        /*
         * These first tests check bounds on the DateTimeHelper FromTimeslot method.
         * Helpful ref: https://www.epochconverter.com/weeks/2024 to get week numbers
         */
        [Fact]
        public void FromTimeslotIsCorrect()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Monday,
                Time = new TimeOnly(14, 00)
            };
            var weekNum = 1;
        
            Assert.Equal(new DateTime(2024,01,01, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void FromTimeslotIsCorrect_2()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Sunday,
                Time = new TimeOnly(23, 59, 59, 99)
            };
            var weekNum = 1;
        
            Assert.Equal(new DateTime(2024,01,07, 23,59, 59, 99), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void FromTimeslotIsCorrect_3()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Monday,
                Time = new TimeOnly(00, 00)
            };
            var weekNum = 2;
        
            Assert.Equal(new DateTime(2024,01,08, 00,00, 00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void FromTimeslotIsCorrect_4()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Friday,
                Time = new TimeOnly(14, 00)
            };
            var weekNum = 10;
        
            Assert.Equal(new DateTime(2024,03,08, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void FromTimeslotIsCorrect_5()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Monday,
                Time = new TimeOnly(14, 00)
            };
            var weekNum = 53;
        
            Assert.Equal(new DateTime(2024,12,30, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void FromTimeslotIsCorrect_6()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Wednesday,
                Time = new TimeOnly(14, 00)
            };
            var weekNum = 53;
        
            Assert.Equal(new DateTime(2025,01,01, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }

        [Fact]
        public void ExplicitWeekNumHasPriority()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Thursday,
                Time = new TimeOnly(14, 00),
                WeekNum = 2
            };
            var weekNum = 3;
        
            Assert.Equal(new DateTime(2024,01,18,14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot, weekNum));
        }
    
        [Fact]
        public void ExplicitWeekNumIsApplied()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Thursday,
                Time = new TimeOnly(14, 00),
                WeekNum = 2
            };
        
            Assert.Equal(new DateTime(2024,01,11,14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot));
        }

        [Fact]
        public void ExceptionThrowsForMissingWeek()
        {
            var timeslot = new TimeslotDto()
            {
                Day = Enums.Day.Thursday,
                Time = new TimeOnly(14, 00)
            };
        
            Assert.Throws<InvalidUserOperationException>(() => DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot));
        }
    
        /*
         * Part 2 - now the reverse. Test GetWeekNumFromDateTime
         * Some tests to catch inaccuracy with edge cases
         */

        [Fact]
        public void FromDateTimeIsCorrect()
        {
            var sampleDate = new DateTime(2024, 01, 01);
            Assert.Equal(1, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_1_1()
        {
            var sampleDate = new DateTime(2024, 01, 01, 00, 00, 01, 01);
            Assert.Equal(1, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_1_2()
        {
            var sampleDate = new DateTime(2024, 01, 07, 23, 59, 59, 99);
            Assert.Equal(1, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_1_3()
        {
            var sampleDate = new DateTime(2024, 01, 08);
            Assert.Equal(2, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_2()
        {
            var sampleDate = new DateTime(2024, 01, 07);
            Assert.Equal(1, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_3()
        {
            var sampleDate = new DateTime(2024, 01, 16);
            Assert.Equal(3, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_4()
        {
            var sampleDate = new DateTime(2024, 01, 16, 23, 59, 59);
            Assert.Equal(3, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        [Fact]
        public void FromDateTimeIsCorrect_5()
        {
            var sampleDate = new DateTime(2024, 01, 16, 00, 00, 01);
            Assert.Equal(3, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
    
        [Fact]
        public void FromDateTimeIsCorrect_6()
        {
            var sampleDate = new DateTime(2024, 07, 22, 20, 00, 00);
            Assert.Equal(30, DateTimeHelper.GetWeekNumFromDateTime(DB_START_DATE, sampleDate));
        }
    
        /*
         * ConvertDotNetDay tests
         */

        [Fact]
        public void DotNetConverts_1()
        {
            Assert.Equal(Enums.Day.Monday, DateTimeHelper.ConvertDotNetDay(DayOfWeek.Monday)); 
        }
    
        [Fact]
        public void DotNetConverts_2()
        {
            Assert.Equal(Enums.Day.Wednesday, DateTimeHelper.ConvertDotNetDay(DayOfWeek.Wednesday)); 
            _testOutputHelper.WriteLine("it's wednesday my dudes");
        }
    
        [Fact]
        public void DotNetConverts_3()
        {
            Assert.Equal(Enums.Day.Saturday, DateTimeHelper.ConvertDotNetDay(DayOfWeek.Saturday)); 
        }
    
        [Fact]
        public void DotNetConverts_4()
        {
            Assert.Equal(Enums.Day.Sunday, DateTimeHelper.ConvertDotNetDay(DayOfWeek.Sunday)); 
        }
    
        /*
         * GetDateRangeStringFromWeekNum tests
         */

        [Theory]
        [InlineData("01/01/2024 - 07/01/2024",  0)] // test abnormal cases
        [InlineData("01/01/2024 - 07/01/2024",  -1)] // test abnormal cases
        [InlineData("01/01/2024 - 07/01/2024",  1)]
        [InlineData("08/01/2024 - 14/01/2024",  2)]
        [InlineData("15/01/2024 - 21/01/2024",  3)]
        [InlineData("22/01/2024 - 28/01/2024",  4)]
        [InlineData("29/01/2024 - 04/02/2024",  5)]
        [InlineData("23/12/2024 - 29/12/2024",  52)]
        [InlineData("24/11/2025 - 30/11/2025",  100)]
        public void DateRangeIsCorrect(string expected, int weekNum)
        {
            Assert.Equal(expected, DateTimeHelper.GetDateRangeStringFromWeekNum(DB_START_DATE, weekNum));
        }
        
        // New Timeslot system tests
        [Theory]
        [InlineData("2024-01-01 00:00",  1, 1)] 
        [InlineData("2024-01-01 00:05",  2, 1)] 
        [InlineData("2024-01-01 00:35",  8, 1)] 
        [InlineData("2024-01-01 12:00",  145, 1)] 
        [InlineData("2024-01-01 23:55",  288, 1)] 
        [InlineData("2024-01-02 00:00",  289, 1)] 
        [InlineData("2024-01-02 12:00",  433, 1 )]  // 12:00 Tue
        [InlineData("2024-01-02 23:55",  576, 1)]  // 23:55 Tue
        [InlineData("2024-01-03 00:00",  577 , 1)]  // 00:00 Wed
        [InlineData("2024-01-03 23:55",  864 , 1)]  // 23:55 Wed
        [InlineData("2024-01-04 00:00",  865 , 1)]  // 00:00 Thur
        [InlineData("2024-01-04 23:55",  1152 , 1)]  // 23:55 Thur
        [InlineData("2024-01-05 00:00",  1153 , 1)]  // 00:00 Fri
        [InlineData("2024-01-05 23:55",  1440 , 1)]  // 23:55 Fri
        [InlineData("2024-01-06 00:00",  1441 , 1)]  // 00:00 Sat
        [InlineData("2024-01-06 23:55",  1728 , 1)]  // 23:55 Sat
        [InlineData("2024-01-07 00:00",  1729 , 1)]  // 00:00 Sun
        [InlineData("2024-01-07 23:55",  2016, 1)]  // 22:55 Sun
        [InlineData("2024-01-08 00:00",  1, 2)] 
        [InlineData("2024-01-10 00:00",  577, 2)] 
        [InlineData("2024-01-10 01:40",  597, 2)] 
        [InlineData("2024-12-29 22:40",  2001, 52)] 
        [InlineData("2024-12-29 23:55",  2016, 52)] 
        public void TimeslotDateIsCorrect(string expected, int tsIdx, int weekNum)
        {
            Assert.Equal(expected, DateTimeHelper.FromTimeslotIdx(DB_START_DATE, tsIdx, weekNum).ToString("yyyy-MM-dd HH:mm"));
        }
        
        // check exception throws for out of range values
        [Theory]
        [InlineData( 0, 1)] 
        [InlineData( 1, 0)] 
        [InlineData( -1, 1)] 
        [InlineData( -101, -1)] 
        [InlineData( 2017, 1)] 
        [InlineData( 100000, 1)] 
        [InlineData( 100000, 293940234)] 
        [InlineData( 100000, -293940234)] 
        [InlineData( 1, -293940234)] 
        public void TimeslotDateIsIncorrect(int tsIdx, int weekNum)
        {
            Assert.Throws<InvalidUserOperationException>(() => DateTimeHelper.FromTimeslotIdx(DB_START_DATE, tsIdx, weekNum));
        }
        
        // Test GetTime and GetDay for edge cases
        [Theory]
        [InlineData("00:00",  1 )]  // 00:00 Mon
        [InlineData("12:00",  145 )]  // 12:00 Mon
        [InlineData("23:55",  288 )]  // 23:55 Mon
        [InlineData("00:00",  289 )]  // 00:00 Tue
        [InlineData("12:00",  433 )]  // 12:00 Tue
        [InlineData("23:55",  576 )]  // 23:55 Tue
        [InlineData("00:00",  577 )]  // 00:00 Wed
        [InlineData("23:55",  864 )]  // 23:55 Wed
        [InlineData("00:00",  865 )]  // 00:00 Thur
        [InlineData("23:55",  1152 )]  // 23:55 Thur
        [InlineData("00:00",  1153 )]  // 00:00 Fri
        [InlineData("23:55",  1440 )]  // 23:55 Fri
        [InlineData("00:00",  1441 )]  // 00:00 Sat
        [InlineData("23:55",  1728 )]  // 23:55 Sat
        [InlineData("00:00",  1729 )]  // 00:00 Sun
        [InlineData("23:55",  2016 )]  // 23:55 Sun
        public void GetTimeIsCorrect(string expected, int tsIdx)
        {
            Assert.Equal(expected, TimeslotHelper.GetTime(tsIdx).ToString("HH:mm"));
        }
        
        [Theory]
        [InlineData(Enums.Day.Monday,  1 )]  // 00:00 Mon
        [InlineData(Enums.Day.Monday,  145 )]  // 12:00 Mon
        [InlineData(Enums.Day.Monday,  288 )]  // 23:55 Mon
        [InlineData(Enums.Day.Tuesday,  289 )]  // 00:00 Tue
        [InlineData(Enums.Day.Tuesday,  433 )]  // 12:00 Tue
        [InlineData(Enums.Day.Tuesday,  576 )]  // 23:55 Tue
        [InlineData(Enums.Day.Wednesday,  577 )]  // 00:00 Wed
        [InlineData(Enums.Day.Wednesday,  864 )]  // 23:55 Wed
        [InlineData(Enums.Day.Thursday,  865 )]  // 00:00 Thur
        [InlineData(Enums.Day.Thursday,  1152 )]  // 23:55 Thur
        [InlineData(Enums.Day.Friday,  1153 )]  // 00:00 Fri
        [InlineData(Enums.Day.Friday,  1440 )]  // 23:55 Fri
        [InlineData(Enums.Day.Saturday,  1441 )]  // 00:00 Sat
        [InlineData(Enums.Day.Saturday,  1728 )]  // 23:55 Sat
        [InlineData(Enums.Day.Sunday,  1729 )]  // 00:00 Sun
        [InlineData(Enums.Day.Sunday,  2016 )]  // 23:55 Sun
        public void GetDayIsCorrect(Enums.Day expected, int tsIdx)
        {
            Assert.Equal(expected, TimeslotHelper.GetDay(tsIdx));
        }
    }
}