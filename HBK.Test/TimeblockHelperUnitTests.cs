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
    public class TimeblockHelperUnitTests(ITestOutputHelper _testOutputHelper)
    {
        public static string DB_START_DATE = "2024-01-01";

        // test the difference
        [Fact]
        public void DifferenceReturnsExpectedFinalAvailability_WithConsecutiveAppointments()
        {
            var availability = new List<TimeblockDto>()
            {
                new() {StartTick = 103, EndTick = 139}, // Mon 8.30 -> 11.30
                new() {StartTick = 151, EndTick = 175}, // Mon 12.30 -> 14.30
                new() {StartTick = 181, EndTick = 217}, // Mon 15.00 -> 18.00
            };
            var appointments = new List<TimeblockDto>()
            {
                new() {StartTick = 109, EndTick = 112}, // Mon 9.00 -> 9.15
                new() {StartTick = 112, EndTick = 115}, // Mon 9.15 -> 9.30
                new() {StartTick = 121, EndTick = 123}, // Mon 10.00 -> 10:10
                new() {StartTick = 136, EndTick = 139}, // Mon 11:15 -> 11:30
                new() {StartTick = 151, EndTick = 154}, // Mon 12:30 -> 12:45
                new() {StartTick = 157, EndTick = 160}, // Mon 13:00 -> 13:15
            };
            // availability - appointments = finalAva
            var finalAvailability = new List<TimeblockDto>()
            {
                new() {StartTick = 103, EndTick = 109}, // Mon 8.30 -> 9.00
                new() {StartTick = 115, EndTick = 121}, // Mon 9.15 -> 10.00
                new() {StartTick = 123, EndTick = 136}, // Mon 10.10 -> 11.15
                new() {StartTick = 154, EndTick = 157}, // Mon 12.45 -> 13.00
                new() {StartTick = 160, EndTick = 175}, // Mon 13.15 -> 14.30
                new() {StartTick = 181, EndTick = 217}, // Mon 15.00 -> 1800
            };
            
            // Act
            availability = availability.Difference(appointments);
            
            // Assert
            Assert.Equivalent(finalAvailability, availability, true);
        }
        
        [Fact]
        public void DifferenceReturnsExpectedFinalAvailability_AcrossDays()
        {
            var availability = new List<TimeblockDto>()
            {
                new() {StartTick = 103, EndTick = 217}, // Mon 8.30 -> 16.00
                new() {StartTick = 433, EndTick = 469}, // Tues 12.00 -> 15.00
                new() {StartTick = 697, EndTick = 769}, // Wed 10.00 -> 16.00
            };
            var appointments = new List<TimeblockDto>()
            {
                new() {StartTick = 124, EndTick = 127}, // Mon 10.15 -> 10.30
                new() {StartTick = 130, EndTick = 133}, // Mon 10.45 -> 11.00
                new() {StartTick = 151, EndTick = 154}, // Mon 12.30 -> 12.45
            };
            // availability - appointments = finalAva
            var finalAvailability = new List<TimeblockDto>()
            {
                new() {StartTick = 103, EndTick = 124}, // Mon 8.30 -> 10.15
                new() {StartTick = 127, EndTick = 130}, // Mon 10.30 -> 10.45
                new() {StartTick = 133, EndTick = 151}, // Mon 11.00 -> 12.30
                new() {StartTick = 154, EndTick = 217}, // Mon 12.30 -> 16.00
                new() {StartTick = 433, EndTick = 469}, // Tues 12.00 -> 15.00
                new() {StartTick = 697, EndTick = 769}, // Wed 10.00 -> 16.00
            };
            
            // Act
            availability = availability.Difference(appointments);
            
            // Assert
            Assert.Equivalent(finalAvailability, availability, true);
        }

        [Fact]
        public void TestEquality()
        {
            var tb1 = new TimeblockDto() { StartTick = 100, EndTick = 200 };
            var tb2 = new TimeblockDto() { StartTick = 100, EndTick = 200 };
            Assert.Equal(tb1, tb2);
        }
        
        [Fact]
        public void TestEquality_2()
        {
            var tb1 = new TimeblockDto() { StartTick = 100, EndTick = 200, Duration = 100};
            var tb2 = new TimeblockDto() { StartTick = 100, EndTick = 200, Duration = 0}; // malformed!
            Assert.NotEqual(tb1, tb2);
        }

        [Fact]
        public void TestMergingOfConsecutiveBlocks_DoesNotMerge()
        {
            var tbListBefore = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 140},
                new() {StartTick = 145, EndTick = 150},
                new() {StartTick = 160, EndTick = 170}
            };
            var tbListAfter = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 140},
                new() {StartTick = 145, EndTick = 150},
                new() {StartTick = 160, EndTick = 170}
            };
            tbListBefore = tbListBefore.FlattenTimeblocks();
            Assert.Equivalent(tbListAfter, tbListBefore, true);
        }
        
        [Fact]
        public void TestMergingOfConsecutiveBlocks_DoesMerge()
        {
            var tbListBefore = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 145},
                new() {StartTick = 145, EndTick = 150},
            };
            var tbListAfter = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 150},
            };
            tbListBefore = tbListBefore.FlattenTimeblocks();
            Assert.Equivalent(tbListAfter, tbListBefore, true);
        }
        
        [Fact]
        public void TestMergingOfConsecutiveBlocks_DoesMerge_2()
        {
            var tbListBefore = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 145},
                new() {StartTick = 145, EndTick = 150},
                new() {StartTick = 150, EndTick = 170},
            };
            var tbListAfter = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 170},
            };
            tbListBefore = tbListBefore.FlattenTimeblocks();
            Assert.Equivalent(tbListAfter, tbListBefore, true);
        }
        
        [Fact]
        public void TestMergingOfConsecutiveBlocks_DoesMerge_3()
        {
            var tbListBefore = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 145},
                new() {StartTick = 145, EndTick = 150},
                new() {StartTick = 155, EndTick = 170},
            };
            var tbListAfter = new List<TimeblockDto>()
            {
                new() {StartTick = 120, EndTick = 150},
                new() {StartTick = 155, EndTick = 170},
            };
            tbListBefore = tbListBefore.FlattenTimeblocks();
            Assert.Equivalent(tbListAfter, tbListBefore, true);
        }
    }
}