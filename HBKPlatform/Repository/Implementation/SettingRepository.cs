using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
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
        public async Task<List<SettingDto>> GetAllTenancySettings()
        {
            return await _db.Settings.Select(x => new SettingDto()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value,
                Value2 = x.Value2
            }).ToListAsync();
        }

        public async Task Update(SettingDto setting)
        {
            var dbSetting = await _db.Settings.FirstOrDefaultAsync(x => x.Key == setting.Key) ??
                            throw new IdxNotFoundException($"Setting key {setting.Key} not found in DB");
            dbSetting.Value = setting.Value;
            dbSetting.Value2 = setting.Value2;
            await _db.SaveChangesAsync();
        }
        
        public async Task Create(SettingDto setting)
        {
            if (_db.Settings.Any(x => x.Key == setting.Key))
            {
                throw new DuplicateKeyException("Duplicate settings keys are not permitted.");
            }
            
            await _db.AddAsync(new Setting()
            {
                Key = setting.Key,
                Value = setting.Value,
                Value2 = setting.Value2
            });
            
            await _db.SaveChangesAsync();
        }
    
    }
}