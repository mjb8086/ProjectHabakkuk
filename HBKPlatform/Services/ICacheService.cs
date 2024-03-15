using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services
{
    public interface ICacheService
    {
        public string GetPractitionerName(int practitionerId);
        public string GetClientName(int clientId);
        public int GetLeadPractitionerId(int practiceId);
        public PractitionerDetailsLite GetPractitionerDetailsLite(int pracId);
        public ClientDetailsLite GetClientDetailsLite(int clientId);
        public Task<List<PractitionerDetailsLite>> GetPracticePractitionerDetailsLite();
        public Task<List<ClientDetailsLite>> GetPracticeClientDetailsLite();
        public Task<Dictionary<string, SettingDto>> GetAllTenancySettings();
        public Task<Dictionary<int, TreatmentDto>> GetTreatments();


        // Clearing methods - call after any DB update action on the entities.
        public void ClearPractitionerDetails(int practitionerId);
        public void ClearClientDetails(int clientId);
        public void ClearPracticeClientDetails();
        public void ClearSettings();
        public void ClearTreatments();
        public void ClearAll();
    }
}