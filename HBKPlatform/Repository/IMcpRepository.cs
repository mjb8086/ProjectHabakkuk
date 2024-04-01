using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Repository
{
    public interface IMcpRepository
    {
        public Task<PracticeDetailsDto> GetPracticeAlone(int practiceId);
        public Task<List<PracticeDetailsLite>> GetPracticeDetailsLite();
        public Task UpdatePracticeDetails(PracticeDto practice);
        public Task<Tenancy> RegisterPractice(PracticeRegistrationDto practice);
        public Task<List<UserDetailsUac>> GetPracticePracs(int practiceId);
        public Task<Dictionary<int, UserDetailsUac>> GetPractitionerLockoutStatusDict(int practiceId);
        public Task<UserDetailsUac> GetLeadManagerLockoutStatus(int clinicId);
        public Task<List<UserDto>> GetRecentLogins();
        public Task<int> GetRegisteredUserCount();
        public Task<string> GetPracUserId(int pracId);
        public Task<string> GetLeadManagerUserId(int clinicId);
        public Task<string> GetClientUserId(int clientId);
        public Task<List<ClinicLite>> GetClinicDetailsLite();
        public Task<ClinicDetailsDto> GetClinicAlone(int clinicId);
        public Task RegisterClinic(ClinicRegistrationDto clinic);
        public Task UpdateClinicDetails(ClinicDto clinic);
    }
}