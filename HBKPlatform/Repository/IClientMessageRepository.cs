using HBKPlatform.Database;

namespace HBKPlatform.Repository;

public interface IClientMessageRepository
{

    public Task SaveMessage(ClientMessage message);
}