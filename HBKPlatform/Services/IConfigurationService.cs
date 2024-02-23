using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services;

public interface IConfigurationService
{
   public Task<SettingDto> GetSettingOrDefault(string key);
}