using HBKPlatform.Database;
using Microsoft.Extensions.Caching.Memory;

namespace HBKPlatform.Services.Implementation
{
    /// <summary>
    /// HBKPlatform security service.
    /// Uses a cache - be sure to clear it at appropriate times.
    /// 
    /// Author: Mark Brown
    /// Authored: 26/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class SecurityService(ApplicationDbContext _db, ITenancyService _tenancy, IMemoryCache _memoryCache) : ISecurityService
    {
        // Security service - all entries expire after 1 hour
        private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
        private int TenancyId = _tenancy.TenancyId;
    
        /// <summary>
        /// Refresh this when a new client is registered to the practice
        /// Security method, verify that the client Id specified is registered under the practitioner.
        /// Returns TRUE if yes, or false if client is not registered under the prac/prac is not found
        /// </summary>
        public async Task<bool> VerifyClientPracOwnership(int clientId, int pracId)
        {
            string key = $"ClientPracMap-t{TenancyId}";
        
            if (_memoryCache.TryGetValue(key, out ILookup<int, int>? clientPracMap)) 
                return  clientPracMap?[pracId].Contains(clientId) ?? false;
        
            clientPracMap = _db.ClientPractitioners.ToLookup(x => x.PractitionerId, x => x.ClientId);
            _memoryCache.Set(key, clientPracMap, CacheEntryOptions);
            return  clientPracMap[pracId]?.Contains(clientId) ?? false;
        }

        public void ClearClientPracOwnership()
        {
            _memoryCache.Remove($"ClientPracMap-t{TenancyId}");
        }
        
    }
}