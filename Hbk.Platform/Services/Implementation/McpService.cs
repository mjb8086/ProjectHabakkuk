using Hbk.Database;
using Hbk.Models.DTO;
using Hbk.Models.View.MCP;
using Hbk.Platform.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hbk.Platform.Services.Implementation
{
    public class McpService(
        IMcpRepository _mcpRepo,
        ICentralScrutinizerService _css,
        [FromServices] ApplicationDbContext dbContext) : IMcpService
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
            await _mcpRepo.RegisterPractice(model);
        }

        public async Task<UserAccountFunctions> GetUacViewPractices()
        {
            var practices = await _mcpRepo.GetPracticeDetailsLite();
            return new UserAccountFunctions()
            {
                Practices = practices.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList(),
            };
        }
        
        public async Task<UserAccountFunctions> GetUacViewClinic()
        {
            var clinics = await _mcpRepo.GetClinicDetailsLite();
            return new UserAccountFunctions()
            {
                Clinics = clinics.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList(),
            };
        }

        public async Task<UacUserSelect> GetPracPracs(int practiceId)
        {
            var pracDetailsUac = await _mcpRepo.GetPractitionerLockoutStatusDict(practiceId);
        
            return new UacUserSelect()
            {
                Users = pracDetailsUac
            };
        }
        
        public async Task<UserDetailsUac> GetLeadManager(int clinicId)
        {
            return await _mcpRepo.GetLeadManagerLockoutStatus(clinicId);
        }

        public async Task<List<UserDto>> GetRecentLogins()
        {
            return await _mcpRepo.GetRecentLogins();
        }

        public async Task<SystemStats> GetStatsView()
        {
            var database = dbContext.Database.ProviderName ?? "Unknown provider";
            var host = "Process memory";

            if (dbContext.Database.IsRelational())
            {
                var connection = dbContext.Database.GetDbConnection();
                database = string.IsNullOrWhiteSpace(connection.Database) ? database : connection.Database;
                host = connection.DataSource;
            }
            
            return new SystemStats()
            {
                NumOnline = _css.GetActiveCount(),
                NumRegistered = await _mcpRepo.GetRegisteredUserCount(),
                Db = database,
                Host = host
            };
        }
        
        //////////////////////////////////////////////////////////////////////////////// 
        // CLINIC METHODS
        //////////////////////////////////////////////////////////////////////////////// 
        public async Task<List<ClinicLite>> GetListClinicsView()
        {
            return await _mcpRepo.GetClinicDetailsLite();
        }
        
        public async Task<ClinicDetailsDto> GetClinicModel(int clinicId)
        {
            return await _mcpRepo.GetClinicAlone(clinicId);
        }
        
        public async Task RegisterClinic(ClinicRegistrationDto model)
        {
            await _mcpRepo.RegisterClinic(model);
        }
        
        public async Task UpdateClinic(ClinicDto model)
        {
            await _mcpRepo.UpdateClinicDetails(model);
        }


    }
}
