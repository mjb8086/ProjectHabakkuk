using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class ClientDetailsService(IClientRepository _clientRepo, IUserService _userService, 
    ICacheService _cacheService) : IClientDetailsService
{
    public async Task<AllClients> GetAllClientsView()
    {
        return new AllClients() { Clients = await _clientRepo.GetLiteDetails()};
    }

    public async Task<ClientDto> GetClient(int clientId)
    {
        // todo : security checks
        return _clientRepo.GetClientDetails(clientId);
    }
    
    public async Task<ClientDto> GetClientAsClient()
    {
        // todo : security checks
        var clientId = _userService.GetClaimFromCookie("ClientId");
        return _clientRepo.GetClientDetails(clientId);
    }

    public async Task UpdateClientDetails(ClientDto client)
    {
        await _clientRepo.Update(client);
        _cacheService.ClearClientDetails(client.Id);
        _cacheService.ClearClinicClientDetails();
    }

    public async Task UpdateClientDetailsAsClient(ClientDto client)
    {
        // todo: security checks
        client.Id = _userService.GetClaimFromCookie("ClientId");
        await _clientRepo.Update(client);
        _cacheService.ClearClientDetails(client.Id);
        _cacheService.ClearClinicClientDetails();
    }

    public async Task CreateClient(ClientDto client)
    {
        client.ClinicId = _userService.GetClaimFromCookie("ClinicId");
        await _clientRepo.Create(client);
        _cacheService.ClearClinicClientDetails();
    }

    public ClientDetailsIndex GetClientDetailsIndex()
    {
        return new ClientDetailsIndex() { NumClients = _clientRepo.GetClientCount() };
    }
}