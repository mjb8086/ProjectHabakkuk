using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services;

public interface ICacheService
{
    public string GetPracName(int pracId);
    public string GetClientName(int clientId);
    public int GetLeadPracId(int clinicId);
    public PracDetailsLite GetPracDetailsLite(int pracId);
    public ClientDetailsLite GetClientDetailsLite(int clientId);
    public Task<List<PracDetailsLite>> GetClinicPracDetailsLite();
    public Task<List<ClientDetailsLite>> GetClinicClientDetailsLite();
    public Task<bool> VerifyClientClinicMembership(int clientId, int clinicId);
    public Task<Dictionary<string, SettingDto>> GetAllTenancySettings();
    public Task<Dictionary<int, TreatmentDto>> GetTreatments();
}