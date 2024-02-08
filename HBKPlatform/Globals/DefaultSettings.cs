using HBKPlatform.Models.DTO;

namespace HBKPlatform.Globals;

public class DefaultSettings
{
    public const int DEFAULT_PASSWORD_LENGTH = 14;
    public static readonly Dictionary<string, SettingDto> DefaultSetting = new()
    {
        { "DbStartDate", new SettingDto() { Value = "2024-01-01" }},
        { "BookingAdvanceWeeks", new SettingDto() { Value = "2" }}
    };
}