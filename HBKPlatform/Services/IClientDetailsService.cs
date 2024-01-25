using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services;

public interface IClientDetailsService
{
    public Task<AllClients> GetAllClientsView();
    public Task<ClientDto> GetClient(int clientId);
}