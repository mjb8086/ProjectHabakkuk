using HBKPlatform.Models.DTO;

namespace HBKPlatform.Globals
{
    public struct DefaultSettings
    {
        public const int DEFAULT_PASSWORD_LENGTH = 14;
        public const bool LOCKOUT_ON_FAILURE = true;
        public const int LOCKOUT_MAX_ATTEMPTS = 5;
        public const int AVAILABILITY_ADVANCE_WEEKS = 52;

        public const string LOG_ROOT_PROD = "/var/log/";
        public const string LOG_ROOT_LOCAL = "./log/";
        
        public static readonly Dictionary<string, SettingDto> DefaultSetting = new()
        {
            { "DbStartDate", new SettingDto() { Value = "2024-01-01" }},
            { "BookingAdvanceWeeks", new SettingDto() { Value = "2" }}
        };
    }

    public struct Consts
    {
        public const string VERSION = "1.0";
        public const string RELEASE_DATE = "01/03/2024";
        public const string HBK_NAME =
            "    __  ______  __ __    ____  __      __  ____                   \n   / / / / __ )/ //_/   / __ \\/ /___ _/ /_/ __/___  _________ ___ \n  / /_/ / __  / ,<     / /_/ / / __ `/ __/ /_/ __ \\/ ___/ __ `__ \\\n / __  / /_/ / /| |   / ____/ / /_/ / /_/ __/ /_/ / /  / / / / / /\n/_/ /_/_____/_/ |_|  /_/   /_/\\__,_/\\__/_/  \\____/_/  /_/ /_/ /_/ \n";

    }
}