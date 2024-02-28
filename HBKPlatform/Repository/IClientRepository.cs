using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IClientRepository
    {
        public ClientDto GetClientDetails(int clientId);
        public Task<List<ClientDetailsLite>> GetLiteDetails();
        public Task Create(ClientDto client);
        public Task Update(ClientDto client);
        public int GetClientCount();
    }
}