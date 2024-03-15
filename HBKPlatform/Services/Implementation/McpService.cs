using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBKPlatform.Services.Implementation
{
    public class McpService(IMcpRepository _mcpRepo, ITimeslotRepository _timeslotRepo, ICacheService _cacheService): IMcpService
    {
    
        /* MCP Methods */
        public async Task<PracticeDetailsDto> GetPracticeModel(int practiceId)
        {
            return await _mcpRepo.GetPracticeAlone(practiceId);
        }

        public async Task<ListPractices> GetListPracticesView()
        {
            return new ListPractices() { Practices = await _mcpRepo.GetPracticeDetailsLite() };
        }

        public async Task UpdatePractice(PracticeDto model)
        {
            await _mcpRepo.UpdatePracticeDetails(model);
        }

        public async Task RegisterPractice(PracticeRegistrationDto model)
        {
            var tenancy = await _mcpRepo.RegisterPractice(model);
            await _timeslotRepo.Create(TimeslotHelper.GenerateDefaultTimeslots(tenancy));
        }

        public async Task<UserAccountFunctions> GetUacView()
        {
            var practices = await _mcpRepo.GetPracticeDetailsLite();
            return new UserAccountFunctions()
            {
                Practices = practices.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList(),
            };
        }

        public async Task<PracticePractitioners> GetPracPracs(int practiceId)
        {
            var pracDetailsUac = await _mcpRepo.GetPractitionerLockoutStatusDict(practiceId);
        
            return new PracticePractitioners()
            {
                Pracs = pracDetailsUac
            };
        }

        public async Task<List<UserDto>> GetRecentLogins()
        {
            return await _mcpRepo.GetRecentLogins();
        }

    }
}