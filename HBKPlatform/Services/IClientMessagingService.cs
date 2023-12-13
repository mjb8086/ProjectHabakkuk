using HBKPlatform.Database;
using HBKPlatform.Models;

namespace HBKPlatform.Services;

public interface IClientMessagingService
{
     public Task SendMessage(ClientMessage message);
     public Task<ClientMessageConversationModel> GetConversation(string sender, string recipient, int max = 0);

}