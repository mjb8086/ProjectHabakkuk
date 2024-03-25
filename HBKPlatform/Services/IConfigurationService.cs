using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services
{
   public interface IConfigurationService
   {
      public Task<SettingDto> GetSettingOrDefault(string key);
      public Task<bool> IsSettingEnabled(string key);
      public Task UpdateSetting(string key, string value);
      public Task CreateSetting(SettingDto setting);
   }
}