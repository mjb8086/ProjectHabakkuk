using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Services
{
    public interface IMcpService
    {
    
        public Task<ClinicDetailsDto> GetClinicModel(int clinicId);
        public Task<ListClinics> GetListClinicsView();
        public Task UpdateClinic(ClinicDto model);
        public Task RegisterClinic(ClinicRegistrationDto model);
        public Task<UserAccountFunctions> GetUacView();
        public Task<List<UserDto>> GetRecentLogins();
    
        // API Methods
        public Task<ClinicPracs> GetClinicPracs(int clinicId);
    }
}