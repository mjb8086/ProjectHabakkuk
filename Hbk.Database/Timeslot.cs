using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hbk.Common.Globals;
using Hbk.Models.DTO;

namespace Hbk.Database
{
    /// <summary>
    /// Hbk.Platform Timeslot entity.
    /// Intended for use across all tenancies.
    /// 
    /// Author: Mark Brown
    /// Authored: 03/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class Timeslot
    {
        [Key, Column(Order = 1)]
        public int Id { get; set; } 
        public Enums.Day Day { get; set; }
        public TimeOnly Time { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }

        public TimeslotDto ToDto()
        {
            return new()
            {
                Description = Description,
                Id = Id,
                Day = Day,
                Time = Time
            };
        }
    }
}