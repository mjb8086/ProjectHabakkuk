using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface IClientRepository
{
    public ClientDetailsLite GetLiteDetails(int clientId);
}