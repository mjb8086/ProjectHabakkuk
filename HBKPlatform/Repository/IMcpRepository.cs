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
        public Task<List<PractitionerDetailsUac>> GetPracticePracs(int practiceId);
        public Task<Dictionary<int, PractitionerDetailsUac>> GetPractitionerLockoutStatusDict(int practiceId);
        public Task<List<UserDto>> GetRecentLogins();
    }
}