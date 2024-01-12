using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// HBKPlatform setting repository.
/// 
/// Author: Mark Brown
/// Authored: 12/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class SettingRepository (ApplicationDbContext _db) : ISettingRepository
{
    public async Task<List<SettingDto>> GetAllClinicSettings(int clinicId)
    {
        return await _db.Settings.Where(x => x.ClinicId == clinicId).Select(x => new SettingDto()
        {
            Id = x.Id,
            Key = x.Key,
            Value = x.Value,
            Value2 = x.Value2
        }).ToListAsync();
    }
    
}