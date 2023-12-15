using HBKPlatform.Globals;
using HBKPlatform.Models;

namespace HBKPlatform.Repository;

public interface IClientMessageRepository
{

    public Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId, int clinicId,
        int next = 0);

    public Task SaveMessage(int practitionerId, int clientId, int clinicId, string messageBody,
        Enums.MessageOrigin messageOrigin);
}