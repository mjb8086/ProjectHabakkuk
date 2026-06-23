using Hbk.Common.Globals;
using Hbk.Models;
using Hbk.Models.DTO;

namespace Hbk.Platform.Repository
{
    public interface IClientMessageRepository
    {

        public Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId,
            int next = 10);

        public Task SaveMessage(int practitionerId, int clientId, string messageBody,
            Enums.MessageOrigin messageOrigin);

        public Task<int> GetUnreadMessagesAsClient(int pracId, int clientId);
        public Task<int> GetUnreadMessagesAsPractitioner(int pracId);
        public Task<List<UnreadMessageDetailLite>> GetUnreadMessageDetailsAsPractitioner(int pracId);
        public Task UpdateReadReceiptsPractitioner(int clientId, int pracId);
        public Task UpdateReadReceiptsClient(int clientId, int pracId);
    }
}