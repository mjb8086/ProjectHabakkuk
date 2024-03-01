using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Repository
{
    public interface IMcpRepository
    {
        public Task<ClinicDetailsDto> GetClinicAlone(int clinicId);
        public Task<List<ClinicDetailsLite>> GetClinicDetailsLite();
        public Task UpdateClinicDetails(ClinicDto clinic);
        public Task<Tenancy> RegisterClinic(ClinicRegistrationDto clinic);
        public Task<List<PracDetailsUac>> GetClinicPracs(int clinicId);
        public Task<Dictionary<int, PracDetailsUac>> GetPracLockoutStatusDict(int clinicId);
        public Task<List<UserDto>> GetRecentLogins();
    }
}