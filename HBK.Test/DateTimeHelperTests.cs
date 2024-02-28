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
         *
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
        
            Assert.Throws<InvalidOperationException>(() => DateTimeHelper.FromTimeslot(DB_START_DATE, timeslot));
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
    }
}