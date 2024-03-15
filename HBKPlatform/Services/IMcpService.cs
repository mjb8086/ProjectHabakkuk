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
        public Task<UserAccountFunctions> GetUacView();
        public Task<List<UserDto>> GetRecentLogins();
    
        // API Methods
        public Task<PracticePractitioners> GetPracPracs(int practiceId);
    }
}