using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform Cache service.
/// Fast lookups for oft'needed data
/// 
/// Author: Mark Brown
/// Authored: 04/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class CacheService(ApplicationDbContext _db, IMemoryCache _memoryCache, ITenancyService _tenancy): ICacheService
{
    // Default policy: All values will be evicted after 1 day
    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
    
    public string GetPracName(int pracId)
    {
        return GetPracDetailsLite(pracId).Name;
    }

    public string GetClientName(int clientId)
    {
        return GetClientDetailsLite(clientId).Name;
    }

    /// <summary>
    /// Get the lead practitioner Id for a clinic
    /// </summary>
    public int GetLeadPracId(int clinicId)
    {
        string key = $"LeadPrac-t{clinicId}";
        if (_memoryCache.TryGetValue(key, out int pracId)) return pracId;
        
        var clinic = _db.Clinics.FirstOrDefault(x => x.Id == clinicId);
        if (clinic == null || !clinic.LeadPractitionerId.HasValue) 
            throw new KeyNotFoundException($"No practitioner for clinicId {clinicId} exists");
        _memoryCache.Set(key, clinic.LeadPractitionerId.Value, CacheEntryOptions);
        return clinic.LeadPractitionerId.Value;
    }

    /// TODO: Refresh when any of these details are updated in DB
    public PracDetailsLite GetPracDetailsLite(int pracId)
    {
        string key = $"Prac-{pracId}";
        if (_memoryCache.TryGetValue(key, out PracDetailsLite pracDetails)) return pracDetails;

        var prac = _db.Practitioners.FirstOrDefault(x => x.Id == pracId);
        if (prac == null) throw new KeyNotFoundException($"No practitioner of id {pracId} exists");
        
        pracDetails = new PracDetailsLite()
            { Id = prac.Id, Name = $"{prac.Forename} {prac.Surname}", ClinicId = prac.ClinicId };
        _memoryCache.Set(key, pracDetails, CacheEntryOptions);
        return pracDetails;
    }
    
    public ClientDetailsLite GetClientDetailsLite(int clientId)
    {
        string key = $"Client-{clientId}";
        if (_memoryCache.TryGetValue(key, out ClientDetailsLite clientDetails)) return clientDetails;

        var client = _db.Clients.FirstOrDefault(x => x.Id == clientId);
        if (client == null) throw new KeyNotFoundException($"No client of id {clientId} exists");
        
        clientDetails = new ClientDetailsLite()
            { Id = client.Id, Name = $"{client.Forename} {client.Surname}", ClinicId = client.ClinicId };
        _memoryCache.Set(key, clientDetails, CacheEntryOptions);
        return clientDetails;
    }

    public async Task<List<PracDetailsLite>> GetClinicPracDetailsLite()
    {
        string key = $"Pracs-t{_tenancy.TenancyId}";
        if (_memoryCache.TryGetValue(key, out List<PracDetailsLite>? pracDetails)) return pracDetails;
        
        pracDetails = await _db.Practitioners.Select(x => new PracDetailsLite()
            { Id = x.Id, Name = $"{x.Title} {x.Forename} {x.Surname}", ClinicId = x.ClinicId }).ToListAsync();
        _memoryCache.Set(key,  pracDetails, CacheEntryOptions);
        return pracDetails;
    }

    public async Task<List<ClientDetailsLite>> GetClinicClientDetailsLite()
    {
        string key = $"ClinicClients-t{_tenancy.TenancyId}";
        if (_memoryCache.TryGetValue(key, out List<ClientDetailsLite>? clientDetails)) return clientDetails;
        
        clientDetails = await _db.Clients.Select(x => new ClientDetailsLite()
            { Id = x.Id, Name = $"{x.Forename} {x.Surname}", ClinicId = x.ClinicId }).ToListAsync();
        _memoryCache.Set(key,  clientDetails, CacheEntryOptions);
        return clientDetails;
    }

    /// Deprecated?
    /// <summary>
    /// TODO: Refresh this when a new client is registered to the clinic
    /// Security method, verify that the client Id specified is registered under the clinic.
    /// </summary>
    public async Task<bool> VerifyClientClinicMembership(int clientId, int clinicId)
    {
        const string key = "ClinicClientIdMap";
        if (_memoryCache.TryGetValue(key, out Dictionary<int, int>? clientIdMap))
        {
            return clientIdMap?[clientId] == clinicId;
        }
        
        clientIdMap = await _db.Clients.ToDictionaryAsync(x => x.Id, x => x.ClinicId);
        _memoryCache.Set(key,  clientIdMap, CacheEntryOptions);
        return clientIdMap[clientId] == clinicId;
    }

    /// TODO: refresh cache when settings change
    public async Task<Dictionary<string, SettingDto>> GetAllTenancySettings()
    {
        string key = $"Settings-t{_tenancy.TenancyId}";
        if (_memoryCache.TryGetValue(key, out Dictionary<string, SettingDto>? tenancySettings))
        {
            return tenancySettings ?? new Dictionary<string, SettingDto>();
        }
        
        // Ensure duplicate keys may not be created per-Clinic in Settings Repo!!!
        tenancySettings = await _db.Settings.Select(x => new SettingDto()
        {
            Id = x.Id,
            Key = x.Key,
            Value = x.Value,
            Value2 = x.Value2
        }).ToDictionaryAsync(x => x.Key);
        _memoryCache.Set(key,  tenancySettings, CacheEntryOptions);
        return tenancySettings;
    }

    /// <summary>
    /// todo: refresh when treatments are changed
    /// </summary>
    public async Task<Dictionary<int, TreatmentDto>> GetTreatments()
    {
        string key = $"Treatments-t{_tenancy.TenancyId}";
        if (_memoryCache.TryGetValue(key, out Dictionary<int, TreatmentDto>? treatments))
        {
            return treatments ?? new Dictionary<int, TreatmentDto>();
        }
        
        treatments = await _db.Treatments.Select(x => new TreatmentDto()
        {
            Id = x.Id,
            Title = x.Title,
            Cost = x.Cost,
            Requestability = x.TreatmentRequestability
        }).ToDictionaryAsync(x => x.Id);
        _memoryCache.Set(key,  treatments, CacheEntryOptions);
        return treatments;
    }


}