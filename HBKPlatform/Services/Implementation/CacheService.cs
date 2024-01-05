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
public class CacheService(ApplicationDbContext _db, IMemoryCache _memoryCache): ICacheService
{
    // Default policy: All values will be evicted after 1 day
    private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
    
    public string GetPracName(int pracId)
    {
        return GetPracDetailsLite(pracId).Name;
    }

    public string GetClientName(int clientId)
    {
        return GetClientDetailsLite(clientId).Name;
    }

    // TODO: Refresh when all these details are updated in DB
    public PracDetailsLite GetPracDetailsLite(int pracId)
    {
        string key = $"Prac-{pracId}";
        if (_memoryCache.TryGetValue(key, out PracDetailsLite pracDetails)) return pracDetails;

        var prac = _db.Practitioners.FirstOrDefault(x => x.Id == pracId);
        if (prac == null) throw new KeyNotFoundException($"No practitioner of id {pracId} exists");
        
        pracDetails = new PracDetailsLite()
            { Id = prac.Id, Name = $"{prac.Forename} {prac.Surname}", ClinicId = prac.ClinicId };
        _memoryCache.Set(key, pracDetails, _cacheEntryOptions);
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
        _memoryCache.Set(key, clientDetails, _cacheEntryOptions);
        return clientDetails;
    }

    public async Task<List<PracDetailsLite>> GetClinicPracDetailsLite(int clinicId)
    {
        string key = $"ClinicPracs-{clinicId}";
        if (_memoryCache.TryGetValue(key, out List<PracDetailsLite> pracDetails)) return pracDetails;
        
        var practitioners = await _db.Practitioners.Where(x => x.ClinicId == clinicId).ToListAsync();
        
        pracDetails = practitioners.Select(x => new PracDetailsLite()
            { Id = x.Id, Name = $"{x.Title} {x.Forename} {x.Surname}", ClinicId = x.ClinicId }).ToList();
        _memoryCache.Set(key,  pracDetails, _cacheEntryOptions);
        return  pracDetails;
    }

    public async Task<List<ClientDetailsLite>> GetClinicClientDetailsLite(int clinicId)
    {
        string key = $"ClinicClients-{clinicId}";
        if (_memoryCache.TryGetValue(key, out List<ClientDetailsLite> clientDetails)) return clientDetails;
        
        var clients = await _db.Clients.Where(x => x.ClinicId == clinicId).ToListAsync();
        
        clientDetails = clients.Select(x => new ClientDetailsLite()
            { Id = x.Id, Name = $"{x.Forename} {x.Surname}", ClinicId = x.ClinicId }).ToList();
        _memoryCache.Set(key,  clientDetails, _cacheEntryOptions);
        return  clientDetails;
    }

    // TODO: Refresh this when a new client is registertef for the clinic
    /// <summary>
    /// Security method, verify that the client Id specified is registered under the clinic.
    /// </summary>
    public async Task<bool> VerifyClientClinicMembership(int clientId, int clinicId)
    {
        const string key = "ClinicClientIdMap";
        if (_memoryCache.TryGetValue(key, out Dictionary<int, int> clientIdMap))
        {
            return clientIdMap[clientId] == clinicId;
        }
        
        clientIdMap = await _db.Clients.ToDictionaryAsync(x => x.Id, x => x.ClinicId);
        _memoryCache.Set(key,  clientIdMap, _cacheEntryOptions);
        return clientIdMap[clientId] == clinicId;
    }
    
}