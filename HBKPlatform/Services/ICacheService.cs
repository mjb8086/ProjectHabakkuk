using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services;

public interface ICacheService
{
    public string GetPracName(int pracId);
    public string GetClientName(int clientId);
    public PracDetailsLite GetPracDetailsLite(int pracId);
    public ClientDetailsLite GetClientDetailsLite(int clientId);
    public Task<List<PracDetailsLite>> GetClinicPracDetailsLite(int clinicId);
    public Task<List<ClientDetailsLite>> GetClinicClientDetailsLite(int clinicId);
    public Task<bool> VerifyClientClinicMembership(int clientId, int clinicId);
    public Task<Dictionary<string, SettingDto>> GetAllClinicSettings(int clinicId);
}