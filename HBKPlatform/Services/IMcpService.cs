using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Services
{
    public interface IMcpService
    {
    
        public Task<PracticeDetailsDto> GetPracticeModel(int practiceId);
        public Task<ListPractices> GetListPracticesView();
        public Task UpdatePractice(PracticeDto model);
        public Task RegisterPractice(PracticeRegistrationDto model);
        public Task<UserAccountFunctions> GetUacViewPractices();
        public Task<UserAccountFunctions> GetUacViewClinic();
        public Task<List<UserDto>> GetRecentLogins();
        public Task<List<ClinicLite>> GetListClinicsView();
        public Task<ClinicDetailsDto> GetClinicModel(int clinicId);
        public Task RegisterClinic(ClinicRegistrationDto model);
        public Task UpdateClinic(ClinicDto model);
        public Task<SystemStats> GetStatsView();
    
        // API Methods
        public Task<UacUserSelect> GetPracPracs(int practiceId);
        public Task<UserDetailsUac> GetLeadManager(int clinicId);
    }
}