using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IClientRepository
{
    public ClientDto GetClientDetails(int clientId);
    public Task<List<ClientDetailsLite>> GetLiteDetails(int clinicId);
}