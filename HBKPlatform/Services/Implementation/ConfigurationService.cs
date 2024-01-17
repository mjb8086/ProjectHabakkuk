using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services.Implementation;

public class ConfigurationService(ICacheService _cache, IUserService _userService) : IConfigurationService
{
   public async Task<SettingDto> GetSettingOrDefault(string key, int? clinicId)
   {
      clinicId ??= _userService.GetClaimFromCookie("ClinicId");
      var settings = await _cache.GetAllClinicSettings(clinicId.Value);
      return settings.TryGetValue(key, out SettingDto setting) ? setting : DefaultSettings.DefaultSetting[key];
   }
}