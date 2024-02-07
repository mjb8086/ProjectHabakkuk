using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// Clinic Repository.
///
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class ClinicRepository(ApplicationDbContext _db) : IClinicRepository
{
    /// <summary>
    /// Get a Clinic.
    /// </summary>
    /// <returns>Clinic</returns>
    public async Task<ClinicDto> GetClinicAlone(int clinicIdx)
    {
        return await _db.Clinics.Include("LeadPractitioner").Select(x => new ClinicDto()
        {
            OrgName = x.OrgName,
            OrgTagline = x.OrgTagline,
            Email = x.EmailAddress,
            Id = x.Id,
            LicenceStatus = x.LicenceStatus,
            Telephone = x.Telephone,
            RegistrationDate = x.RegistrationDate,
            StreetAddress = x.StreetAddress,
            LeadPrac = new PracDetailsLite() { Name = $"{x.LeadPractitioner.Title} {x.LeadPractitioner.Forename} {x.LeadPractitioner.Surname}", Id = x.LeadPractitionerId }
        }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Could not find clinic ID {clinicIdx}");
    }

    public async Task<List<ClinicDetailsLite>> GetClinicDetailsLite()
    {
        return await _db.Clinics.OrderBy(x => x.Id).Select(x => new ClinicDetailsLite() { Id = x.Id, Name = x.OrgName }).ToListAsync();
    }

    public async Task UpdateClinicDetails(ClinicDto clinic)
    {
        var dbClinic = await _db.Clinics.FirstOrDefaultAsync(x => x.Id == clinic.Id) ?? throw new KeyNotFoundException($"Could not find clinic ID {clinic.Id}");
        dbClinic.OrgName = clinic.OrgName;
        dbClinic.OrgTagline = clinic.OrgTagline;
        dbClinic.Telephone = clinic.Telephone;
        dbClinic.LicenceStatus = clinic.LicenceStatus;
        dbClinic.EmailAddress = clinic.Email;
        dbClinic.StreetAddress = clinic.StreetAddress;
        await _db.SaveChangesAsync();
    }
    
}