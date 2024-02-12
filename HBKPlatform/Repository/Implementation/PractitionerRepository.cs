using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// Practitioner Repository.
///
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class PractitionerRepository(ApplicationDbContext _db) : IPractitionerRepository
{
    public Practitioner GetPractitioner(int mciIdx)
    {
        var practitioner =  _db.Practitioners.First(x => x.Id.Equals(mciIdx));
        return practitioner;
    }

    public async Task<List<PracDetailsUac>> GetClinicPracs(int clinicId)
    {
        return await _db.Practitioners.Where(x => x.ClinicId == clinicId)
            .Select(x => new PracDetailsUac() { Id = x.Id, Name = $"{x.Title}. {x.Forename} {x.Surname}"}).ToListAsync();
    }

    public async Task<Dictionary<int, bool>> GetPracLockoutStatusDict(int clinicId)
    {
        return await _db.Practitioners.Include("User").Where(x => x.UserId != null)
            .ToDictionaryAsync(x => x.Id, x => x.User.LockoutEnabled);
    }

}

