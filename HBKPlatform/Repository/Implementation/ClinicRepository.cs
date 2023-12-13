using HBKPlatform.Database;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

public class ClinicRepository(ApplicationDbContext _db) : IClinicRepository
{
    /// <summary>
    /// Get a Clinic without any relations populated.
    /// </summary>
    /// <returns>Clinic, or Null if not found</returns>
    public async Task<Clinic> GetClinicAlone(int clinicIdx)
    {
        return await _db.Clinics.FirstOrDefaultAsync(x => x.Id == clinicIdx);
    }
    
    /// <summary>
    /// Get clinic including its Practitioner and Client relations populated.
    /// </summary>
    /// <returns>Clinic, or Null if not found</returns>
    public async Task<Clinic> GetCompleteClinic(int clinicIdx)
    {
        return await _db.Clinics.Include("Clients").Include("Practitioner").FirstOrDefaultAsync(x => x.Id == clinicIdx);
    }
}