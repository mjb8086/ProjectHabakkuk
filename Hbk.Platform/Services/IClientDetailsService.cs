using Hbk.Models.DTO;
using Hbk.Models.View.MyND;

namespace Hbk.Platform.Services
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