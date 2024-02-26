using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBKPlatform.Services.Implementation;

public class McpService(IMcpRepository _mcpRepo, ITimeslotRepository _timeslotRepo, ICacheService _cacheService): IMcpService
{
    
    /* MCP Methods */
    public async Task<ClinicDetailsDto> GetClinicModel(int clinicId)
    {
        return await _mcpRepo.GetClinicAlone(clinicId);
    }

    public async Task<ListClinics> GetListClinicsView()
    {
        return new ListClinics() { Clinics = await _mcpRepo.GetClinicDetailsLite() };
    }

    public async Task UpdateClinic(ClinicDto model)
    {
        await _mcpRepo.UpdateClinicDetails(model);
    }

    public async Task RegisterClinic(ClinicRegistrationDto model)
    {
        var tenancy = await _mcpRepo.RegisterClinic(model);
        await _timeslotRepo.Create(TimeslotHelper.GenerateDefaultTimeslots(tenancy));
    }

    public async Task<UserAccountFunctions> GetUacView()
    {
        var clinics = await _mcpRepo.GetClinicDetailsLite();
        return new UserAccountFunctions()
        {
            Clinics = clinics.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList(),
        };
    }

    public async Task<ClinicPracs> GetClinicPracs(int clinicId)
    {
        var lockoutDict = await _mcpRepo.GetPracLockoutStatusDict(clinicId);
        var pracDetailsUac = new Dictionary<int, PracDetailsUac>();
        
        foreach (var prac in (await _mcpRepo.GetClinicPracs(clinicId)))
        {
            prac.HasLockout = lockoutDict[prac.Id];
            pracDetailsUac.Add(prac.Id, prac);
        }
        
        return new ClinicPracs()
        {
            Pracs = pracDetailsUac
        };
    }

}