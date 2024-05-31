using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBK.Test.Dto
{
    /// <summary>
    /// HBKPlatform TimeslotDto unit tests.
    /// 
    /// Author: Mark Brown
    /// Authored: 19/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class TimeslotDtoTests
    {
        [Fact]
        // Verify that the TimeslotDto.Clone() method behaves as expected
        // i.e. returns a populated duplicate of the original and not any references in its members
        public void VerifyTimeslotDuplicate()
        {
            var originalClinicId = 1;
            var originalDay = Enums.Day.Sunday;
            var originalDescription = "Foo";
            var originalDuration = 60;
            var originalTime = new TimeOnly(12, 01);
        
            var timeslot1 = new TimeslotDto()
            {
                Day = originalDay,
                Description = originalDescription,
                DurationMinutes = originalDuration,
                TimeslotId = 100,
                Time = originalTime,
                WeekNum = 27
            };

            var timeslot2 = timeslot1.Clone();
            timeslot2.Day = Enums.Day.Wednesday;
            timeslot2.Description = "Bar";
            timeslot2.DurationMinutes = 40;
            timeslot2.TimeslotId = 101;
            timeslot2.Time = timeslot2.Time.AddHours(5);
            timeslot2.WeekNum = 10;
        
            Assert.NotEqual(timeslot1, timeslot2);
        
            Assert.NotEqual(timeslot1.Day, timeslot2.Day);
            Assert.NotEqual(timeslot1.Description, timeslot2.Description);
            Assert.NotEqual(timeslot1.DurationMinutes, timeslot2.DurationMinutes);
            Assert.NotEqual(timeslot1.TimeslotId, timeslot2.TimeslotId);
            Assert.NotEqual(timeslot1.Time, timeslot2.Time);
            Assert.NotEqual(timeslot1.WeekNum, timeslot2.WeekNum);
        
            Assert.Equal(originalDescription, timeslot1.Description);
            Assert.Equal(originalDay, timeslot1.Day);
            Assert.Equal(originalDuration, timeslot1.DurationMinutes);
            Assert.Equal(originalTime, timeslot1.Time);
        }

        /*
         * Timeslot clash checking tests
         */
        [Fact]
        public void TimeslotIsClash_1()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(15,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(15,15), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.True(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsClash_1_reversed()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(15,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(15,15), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.True(ts2.IsClash(ts1));
        }
    
        [Fact]
        public void TimeslotIsClash_1_self()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(15,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.True(ts1.IsClash(ts1));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_DiffDay()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(15,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(15,15), DurationMinutes = 30, Day = Enums.Day.Tuesday };
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_DiffWeek()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(15,00), DurationMinutes = 30, Day = Enums.Day.Wednesday, WeekNum = 10};
            var ts2 = new TimeslotDto() { Time = new TimeOnly(15,15), DurationMinutes = 30, Day = Enums.Day.Wednesday, WeekNum = 9};
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_EarlierHour_BoundsTouching()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(14,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(14,30), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_LaterHour_BoundsTouching()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(14,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(13,30), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_MuchEarlierHour()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(10,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(14,30), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_1_MuchLaterHour()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(10,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(18,30), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.False(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_2_BigOverlap()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(11,00), DurationMinutes = 120, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(12,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.True(ts1.IsClash(ts2));
        }
    
        [Fact]
        public void TimeslotIsNotClash_2_BigOverlapReversed()
        {
            var ts1 = new TimeslotDto() { Time = new TimeOnly(11,00), DurationMinutes = 120, Day = Enums.Day.Wednesday };
            var ts2 = new TimeslotDto() { Time = new TimeOnly(12,00), DurationMinutes = 30, Day = Enums.Day.Wednesday };
            Assert.True(ts2.IsClash(ts1));
        }

        [Fact]
        public void TimeslotIsNotClash_Any()
        {
            var timeslots = new[]
            {
                new TimeslotDto() { Time = new TimeOnly(10, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(11, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(12, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(14, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(15, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
            };
            var myTs = new TimeslotDto() { Time = new TimeOnly(13, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 };
            Assert.True(myTs.IsNotClashAny(timeslots.ToList()));
        }
    
        [Fact]
        public void TimeslotIsNotClash_Any_IsClash()
        {
            var timeslots = new[]
            {
                new TimeslotDto() { Time = new TimeOnly(10, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(11, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(12, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(13, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(14, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
                new TimeslotDto() { Time = new TimeOnly(15, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 },
            };
            var myTs = new TimeslotDto() { Time = new TimeOnly(13, 00), DurationMinutes = 60, Day = Enums.Day.Wednesday, WeekNum = 3 };
            Assert.False(myTs.IsNotClashAny(timeslots.ToList()));
        }
    }
}