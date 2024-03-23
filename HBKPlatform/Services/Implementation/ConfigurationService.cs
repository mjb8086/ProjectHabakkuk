using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services.Implementation
{
   public class ConfigurationService(ICacheService _cache, ILogger<ConfigurationService> _logger, ITenancyService _tenancy) : IConfigurationService
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
   }
}