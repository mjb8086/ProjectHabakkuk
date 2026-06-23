using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hbk.Platform.Database
{
    /// <summary>
    /// Hbk.Platform setting entity.
    /// 
    /// Author: Mark Brown
    /// Authored: 12/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class Setting : HbkBaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string? Value2 { get; set; }
    }
}