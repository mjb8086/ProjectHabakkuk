using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface ISettingRepository
{
    public Task<List<SettingDto>> GetAllClinicSettings(int clinicId);
}