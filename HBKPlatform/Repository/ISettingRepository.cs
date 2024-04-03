using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface ISettingRepository
    {
        public Task<List<SettingDto>> GetAllTenancySettings();
        public Task Update(SettingDto setting);
        public Task Create(SettingDto setting);
    }
}