using HBKPlatform.Globals;
using HBKPlatform.Models;

namespace HBKPlatform.Repository;

public interface IClientMessageRepository
{

    public Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId, int clinicId,
        int next = 10);

    public Task SaveMessage(int practitionerId, int clientId, int clinicId, string messageBody,
        Enums.MessageOrigin messageOrigin);

    public Task UpdateReadReceiptsPractitioner(int clientId, int pracId);
    public Task UpdateReadReceiptsClient(int clientId, int pracId);
}