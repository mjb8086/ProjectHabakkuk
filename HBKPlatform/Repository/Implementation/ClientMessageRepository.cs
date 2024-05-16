using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// HBKPlatform ClientMessage Repository.
    /// All messaging between practitioners and clients is stored and fetched here.
    ///
    /// Author: Mark Brown
    /// Authored: 13/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class ClientMessageRepository(ApplicationDbContext _db) : IClientMessageRepository
    {
        public const int UNREAD_FETCH_LIMIT = 15;
        
        public async Task SaveMessage(int practitionerId, int clientId, string messageBody,
            Enums.MessageOrigin messageOrigin)
        {
            // PreviousMessageId - null on first message
            var message = new ClientMessage()
            {
                DateOpened = null,
                MessageStatusPractitioner = messageOrigin == Enums.MessageOrigin.Practitioner ? Enums.MessageStatus.Read : Enums.MessageStatus.Unread,
                MessageStatusClient = messageOrigin == Enums.MessageOrigin.Client ? Enums.MessageStatus.Read : Enums.MessageStatus.Unread,
                PreviousMessageId = null,
                PractitionerId = practitionerId,
                ClientId = clientId,
                MessageOrigin = messageOrigin,
                MessageBody = messageBody
            };

            await _db.AddAsync(message);
            _db.SaveChanges();
        }

        public async Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId, int next = 10)
        {
            var convo = new ClientMessageConversationModel();
            var messages = await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == practitionerId  && x.MessageStatusPractitioner != Enums.MessageStatus.Deleted)
                .OrderBy(x => x.Id).Take(next).ToListAsync();

            // TODO: reconsider use of previous and next message Id... may not be needed
            convo.Messages = new List<ClientMessageDto>();
        
            if (messages.Count == 0)
            {
                return convo;
            }

            foreach (var message in messages)
            {
                convo.Messages.Add( new ClientMessageDto
                {
                    MessageOrigin = message.MessageOrigin,
                    MessageBody = message.MessageBody,
                    MessageStatus = message.MessageStatusPractitioner
                });
            }
        
            return convo;
        }

        public async Task<int> GetUnreadMessagesAsClient(int pracId, int clientId)
        {
            return await _db.ClientMessages.CountAsync(x => x.PractitionerId == pracId && x.ClientId == clientId && x.MessageOrigin == Enums.MessageOrigin.Practitioner && x.MessageStatusClient == Enums.MessageStatus.Unread);
        }

        public async Task<int> GetUnreadMessagesAsPractitioner(int pracId)
        {
            return await _db.ClientMessages.CountAsync(x => x.PractitionerId == pracId && x.MessageOrigin == Enums.MessageOrigin.Client && x.MessageStatusPractitioner == Enums.MessageStatus.Unread);
        }

        public async Task<List<UnreadMessageDetailLite>> GetUnreadMessageDetailsAsPractitioner(int pracId)
        {
            // Get count of unread messages for each client, group by client id
            return await _db.ClientMessages.Where(x => x.PractitionerId == pracId && x.MessageOrigin == Enums.MessageOrigin.Client && x.MessageStatusPractitioner == Enums.MessageStatus.Unread)
                .Take(UNREAD_FETCH_LIMIT).GroupBy(x => x.ClientId).Select(g => new UnreadMessageDetailLite()
                    { ClientId = g.Key, UnreadMessageCount = g.Count() }).ToListAsync();
        }

        /// <summary>
        /// Express method to set all messages as 'Read'.
        /// </summary>
        public async Task UpdateReadReceiptsClient(int clientId, int pracId)
        {
            await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == pracId && x.MessageStatusClient == Enums.MessageStatus.Unread)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.MessageStatusClient, Enums.MessageStatus.Read));
        }
    
        /// <summary>
        /// Express method to set all messages as 'Read'.
        /// </summary>
        public async Task UpdateReadReceiptsPractitioner(int clientId, int pracId)
        {
            await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == pracId && x.MessageStatusPractitioner == Enums.MessageStatus.Unread)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.MessageStatusPractitioner, Enums.MessageStatus.Read));
        }
    }
}