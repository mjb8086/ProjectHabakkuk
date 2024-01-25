using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class ClientDetailsService(IClientRepository _clientRepo, IUserService _userService) : IClientDetailsService
{
    public async Task<AllClients> GetAllClientsView()
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        return new AllClients() { Clients = await _clientRepo.GetLiteDetails(clinicId)};
    }

    public async Task<ClientDto> GetClient(int clientId)
    {
        return _clientRepo.GetClientDetails(clientId);
    }
}