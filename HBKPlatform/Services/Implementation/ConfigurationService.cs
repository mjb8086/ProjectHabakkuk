using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation
{
   public class ConfigurationService(ISettingRepository _settingsRepo, ICacheService _cache, ILogger<ConfigurationService> _logger, ITenancyService _tenancy) : IConfigurationService
   {
      public async Task<SettingDto> GetSettingOrDefault(string key)
      {
         var settings = await _cache.GetAllTenancySettings();
         if (settings.TryGetValue(key, out SettingDto? setting))
         {
            return setting;
         }
         _logger.LogWarning($"Could not find setting key {key} in the database for tenancy {_tenancy.TenancyId}. Falling back to default.");
         return DefaultSettings.DefaultSetting[key];
      }

      public async Task<int> GetIntValueOrDefault(string key)
      {
         try
         {
            return Int32.Parse((await GetSettingOrDefault(key)).Value);
         }
         catch (Exception e)
         {
            _logger.LogError($"Integer could not be parsed from Setting value. Message: {e.Message}");
            throw e;
         }
      }

      public async Task<bool> IsSettingEnabled(string key)
      {
         var setting = await GetSettingOrDefault(key);
         return setting.Value is "Yes" or "True" or "Enabled";
      }

      public async Task UpdateSetting(string key, string value)
      {
         // TODO: Use PUT/JSON with this rather than construct key and value
         await _settingsRepo.Update(new SettingDto() { Key = key, Value = value });
         _cache.ClearSettings();
      }
      
      public async Task CreateSetting(SettingDto setting)
      {
         if (!DefaultSettings.IsSettingKeyValid(setting.Key))
         {
            throw new InvalidKeyException(setting.Key);
         }
         await _settingsRepo.Create(setting);
         _cache.ClearSettings();
      }
   }
}