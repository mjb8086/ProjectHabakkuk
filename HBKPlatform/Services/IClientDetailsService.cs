using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services
{
    public interface IClientDetailsService
    {
        public Task<AllClients> GetAllClientsView();
        public Task<ClientDto> GetClient(int clientId);
        public Task UpdateClientDetails(ClientDto client);
        public Task CreateClient(ClientDto client);
        public ClientDetailsIndex GetClientDetailsIndex();
        public Task<ClientDto> GetClientAsClient();
        public Task UpdateClientDetailsAsClient(ClientDto client);
        public Task<List<ClientDetailsLite>> GetClientsLite();
    }
}