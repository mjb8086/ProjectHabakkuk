using System.Text.RegularExpressions;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBKPlatform.Services.Implementation
{
    public class McpService(IMcpRepository _mcpRepo, ICentralScrutinizerService _css, [FromServices] IConfiguration config ): IMcpService
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
            var dbString = config.GetSection("ConnectionStrings").GetSection("HbkContext")?.Value ??
                           "HbkContext not defined?";
            string pattern = @"(Database=([^;]+);)|(Host=([^;]+);)";
            MatchCollection matches = Regex.Matches(dbString, pattern);

            string host = "";
            string database = "";

            foreach (Match match in matches)
            {
                if (match.Groups[4].Success)
                {
                    host = match.Groups[4].Value;
                }
                else if (match.Groups[2].Success)
                {
                    database = match.Groups[2].Value;
                }
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