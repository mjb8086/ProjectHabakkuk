using HBKPlatform.Models.DTO;

namespace HBKPlatform.Globals;

public class DefaultSettings
{
    public static readonly Dictionary<string, SettingDto> DefaultSetting = new()
    {
        { "DbStartDate", new SettingDto() { Value = "2024-01-01" }},
        { "BookingAdvanceWeeks", new SettingDto() { Value = "2" }}
    };
}