using Hbk.Platform.Models;
using Hbk.Platform.Models.DTO;
using Hbk.Platform.Database;

namespace Hbk.Platform.Services
{
     public interface IClientMessagingService
     {
          public Task SendMessage(string messageBody, int recipientId);
          public Task<ClientMessageConversationModel> GetConversationClient(int pracId, int max = 0);
          public Task<ClientMessageConversationModel> GetConversationPractitioner(int clientId, int max = 10);
          public Task<List<UnreadMessageDetailLite>> GetUnreadMessageDetailsAsPractitioner(int? pracId);

     }
}