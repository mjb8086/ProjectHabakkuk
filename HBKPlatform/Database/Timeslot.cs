using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HBKPlatform.Globals;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform Timeslot entity.
    /// Intended for use across all tenancies.
    ///
    /// DEPRECATED
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Index("StartTime")]
    [Index("Day")]
    public class Timeslot
    {
        [Key, Column(Order = 1)]
        public int Id { get; set; } 
        public Enums.Day Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
    }
}