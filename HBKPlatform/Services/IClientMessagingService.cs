using HBKPlatform.Database;
using HBKPlatform.Models;

namespace HBKPlatform.Services;

public interface IClientMessagingService
{
     public Task SendMessage(string messageBody, int recipientId);
     public Task<ClientMessageConversationModel> GetConversationClient(int pracId, int max = 0);
     public Task<ClientMessageConversationModel> GetConversationPractitioner(int clientId, int max = 0);

}