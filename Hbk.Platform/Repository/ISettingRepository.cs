using Hbk.Models.DTO;

namespace Hbk.Platform.Repository
{
    public interface ISettingRepository
    {
        public Task<List<SettingDto>> GetAllTenancySettings();
        public Task Update(SettingDto setting);
        public Task Create(SettingDto setting);
    }
}