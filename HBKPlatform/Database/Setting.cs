namespace HBKPlatform.Database
{
    /// <summary>
    /// HBKPlatform setting entity.
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
        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}