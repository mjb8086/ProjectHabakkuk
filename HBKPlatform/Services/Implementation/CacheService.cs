using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HBKPlatform.Services.Implementation
{
    /// <summary>
    /// HBKPlatform Cache service.
    /// Fast lookups for oft'needed data
    /// 
    /// Author: Mark Brown
    /// Authored: 04/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class CacheService(ApplicationDbContext _db, IMemoryCache _memoryCache, ITenancyService _tenancy, ILogger<CacheService> _logger): ICacheService
    {
        // Default policy: All values will be evicted after 1 day
        private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1));
        private int TenancyId = _tenancy.TenancyId;
    
        public string GetPractitionerName(int practitionerId)
        {
            return GetPractitionerDetailsLite(practitionerId).Name;
        }

        public string GetClientName(int clientId)
        {
            return GetClientDetailsLite(clientId).Name;
        }

        /// <summary>
        /// Get the lead practitioner Id for a practice
        /// </summary>
        public int GetLeadPractitionerId(int practiceId)
        {
            string key = $"LeadPractitioner-t{TenancyId}-p{practiceId}";
            if (_memoryCache.TryGetValue(key, out int pracId)) return pracId;
        
            var practice = _db.Practices.FirstOrDefault(x => x.Id == practiceId);
            if (practice == null || !practice.LeadPractitionerId.HasValue) 
                throw new IdxNotFoundException($"No practitioner for practiceId {practiceId} exists");
            _memoryCache.Set(key, practice.LeadPractitionerId.Value, CacheEntryOptions);
            return practice.LeadPractitionerId.Value;
        }

        /// <summary>
        /// Warning: Cross-Tenancy
        /// </summary>
        public PractitionerDetailsLite GetPractitionerDetailsLite(int pracId)
        {
            string key = $"Practitioner-{pracId}";
            if (_memoryCache.TryGetValue(key, out PractitionerDetailsLite? practitionerDetails)) 
                return practitionerDetails ?? throw new IdxNotFoundException();

            var practitioner = _db.Practitioners.IgnoreQueryFilters().FirstOrDefault(x => x.Id == pracId);
            if (practitioner == null) throw new IdxNotFoundException($"No practitioner of id {pracId} exists");
        
            practitionerDetails = new PractitionerDetailsLite()
                { Id = practitioner.Id, Name = $"{practitioner.Forename} {practitioner.Surname}", PracticeId = practitioner.PracticeId };
            _memoryCache.Set(key, practitionerDetails, CacheEntryOptions);
            return practitionerDetails;
        }
    
        public void ClearPractitionerDetails(int practitionerId)
        {
            _logger.LogDebug($"Clearing practitionerId {practitionerId} details from cache.");
            _memoryCache.Remove($"Practitioner-t{TenancyId}-{practitionerId}");
        }
    
        public ClientDetailsLite GetClientDetailsLite(int clientId)
        {
            string key = $"Client-t{TenancyId}-{clientId}";
            if (_memoryCache.TryGetValue(key, out ClientDetailsLite clientDetails)) return clientDetails;

            var client = _db.Clients.FirstOrDefault(x => x.Id == clientId);
            if (client == null) throw new IdxNotFoundException($"No client of id {clientId} exists");
        
            clientDetails = new ClientDetailsLite()
                { Id = client.Id, Name = $"{client.Forename} {client.Surname}", PracticeId = client.PracticeId };
            _memoryCache.Set(key, clientDetails, CacheEntryOptions);
            return clientDetails;
        }
    
        public void ClearClientDetails(int clientId)
        {
            _logger.LogDebug($"Clearing clientId {clientId} details from cache.");
            _memoryCache.Remove($"Client-t{TenancyId}-{clientId}");
        }

        public async Task<Dictionary<int, PractitionerDetailsLite>> GetPracticePractitionerDetailsLite()
        {
            string key = $"Practitioners-t{TenancyId}";
            if (_memoryCache.TryGetValue(key, out Dictionary<int, PractitionerDetailsLite>? practitionerDetails)) 
                return practitionerDetails ?? throw new IdxNotFoundException();
        
            practitionerDetails = await _db.Practitioners.Select(x => new PractitionerDetailsLite()
                { Id = x.Id, Name = $"{x.Title} {x.Forename} {x.Surname}", PracticeId = x.PracticeId }).ToDictionaryAsync(x => x.Id);
            _memoryCache.Set(key,  practitionerDetails, CacheEntryOptions);
            return practitionerDetails;
        }

        public async Task<List<ClientDetailsLite>> GetPracticeClientDetailsLite()
        {
            string key = $"PracticeClients-t{TenancyId}";
            if (_memoryCache.TryGetValue(key, out List<ClientDetailsLite>? clientDetails)) 
                return clientDetails ?? throw new IdxNotFoundException();
        
            clientDetails = await _db.Clients.Select(x => new ClientDetailsLite()
                { Id = x.Id, Name = $"{x.Forename} {x.Surname}", PracticeId = x.PracticeId }).ToListAsync();
            _memoryCache.Set(key,  clientDetails, CacheEntryOptions);
            return clientDetails;
        }
    
        public void ClearPracticeClientDetails()
        {
            _logger.LogDebug($"Clearing Practice client details from cache.");
            _memoryCache.Remove($"PracticeClients-t{TenancyId}");
        }

        public async Task<Dictionary<string, SettingDto>> GetAllTenancySettings()
        {
            string key = $"Settings-t{TenancyId}";
            if (_memoryCache.TryGetValue(key, out Dictionary<string, SettingDto>? tenancySettings))
            {
                return tenancySettings ?? new Dictionary<string, SettingDto>();
            }
        
            // Ensure duplicate keys may not be created per-Practice in Settings Repo!!!
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
    
        public void ClearSettings()
        {
            _logger.LogDebug($"Clearing settings from cache.");
            _memoryCache.Remove($"Settings-t{TenancyId}");
        }

        /// <summary>
        /// Essential: refresh when treatments are changed
        /// </summary>
        public async Task<Dictionary<int, TreatmentDto>> GetTreatments()
        {
            string key = $"Treatments-t{TenancyId}";
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

        public void ClearTreatments()
        {
            _logger.LogDebug($"Clearing treatments from cache.");
            _memoryCache.Remove($"Treatments-t{TenancyId}");
        }

        /// <summary>
        /// Warning: Cross-Tenancy
        /// </summary>
        public RoomDto GetRoom(int roomId)
        {
            string key = $"Room-r{roomId}";
            if (_memoryCache.TryGetValue(key, out RoomDto? room))
            {
                return room ?? new RoomDto();
            }
            
            room = _db.Rooms.IgnoreQueryFilters().Select(x => new RoomDto() { Id = x.Id, Title = x.Title, Description = x.Description, ClinicId = x.ClinicId }).FirstOrDefault(x => x.Id == roomId);
            if (room == null) 
                throw new IdxNotFoundException($"No room of roomId {roomId} exists");
        
            _memoryCache.Set(key,  room, CacheEntryOptions);
            return room;
        }
        
        public void ClearRoom(int roomId)
        {
            _logger.LogDebug($"Clearing room Id {roomId} from cache.");
            _memoryCache.Remove($"Room-r{roomId}");
        }

        public void ClearAll()
        {
            // todo - test
            if (_memoryCache is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            } 
            _logger.LogWarning("All memory cache contents were cleared.");
        }
    }
}
