using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation
{
    public class ClientDetailsService(IClientRepository _clientRepo, IUserService _userService, 
        ICacheService _cacheService, ISecurityService _securityService) : IClientDetailsService
    {
        public async Task<AllClients> GetAllClientsView()
        {
            return new AllClients() { Clients = await _clientRepo.GetLiteDetails()};
        }

        public async Task<ClientDto> GetClient(int clientId)
        {
            return _clientRepo.GetClientDetails(clientId);
        }
    
        public async Task<ClientDto> GetClientAsClient()
        {
            var clientId = _userService.GetClaimFromCookie("ClientId");
            return _clientRepo.GetClientDetails(clientId);
        }

        public async Task UpdateClientDetails(ClientDto client)
        {
            await _clientRepo.Update(client);
            _cacheService.ClearClientDetails(client.Id);
            _cacheService.ClearPracticeClientDetails();
        }

        public async Task UpdateClientDetailsAsClient(ClientDto client)
        {
            client.Id = _userService.GetClaimFromCookie("ClientId");
        
            await _clientRepo.Update(client);
            // Housekeeping for the cache
            _cacheService.ClearClientDetails(client.Id);
            _cacheService.ClearPracticeClientDetails();
        }

        public async Task CreateClient(ClientDto client)
        {
            client.PracticeId = _userService.GetClaimFromCookie("PracticeId");
            client.PractitionerId = _userService.GetClaimFromCookie("PractitionerId");
        
            await _clientRepo.Create(client);
            // Housekeeping for the cache
            _cacheService.ClearPracticeClientDetails();
            _securityService.ClearClientPracOwnership();
        }

        public ClientDetailsIndex GetClientDetailsIndex()
        {
            return new ClientDetailsIndex() { NumClients = _clientRepo.GetClientCount() };
        }
    }
}