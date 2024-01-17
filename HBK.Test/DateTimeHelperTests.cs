using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using Xunit.Abstractions;

namespace HBK.Test;

/// <summary>
/// HBKPlatform Date Time helper unit tests.
/// 
/// Author: Mark Brown
/// Authored: 16/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// 
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
        
        Assert.Equal(new DateTime(2024,01,01, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
        
        Assert.Equal(new DateTime(2024,01,07, 23,59, 59, 99), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
        
        Assert.Equal(new DateTime(2024,01,08, 00,00, 00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
        
        Assert.Equal(new DateTime(2024,03,08, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
        
        Assert.Equal(new DateTime(2024,12,30, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
        
        Assert.Equal(new DateTime(2025,01,01, 14,00, 00), DateTimeHelper.FromTimeslot(DB_START_DATE, weekNum, timeslot));
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
}