using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

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
    public async Task SaveMessage(int practitionerId, int clientId, int clinicId, string messageBody, Enums.MessageOrigin messageOrigin)
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
            ClinicId = clinicId,
            MessageOrigin = messageOrigin,
            MessageBody = messageBody
        };

        await _db.AddAsync(message);
        _db.SaveChanges();
    }

    public async Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId, int clinicId, int next = 10)
    {
        var convo = new ClientMessageConversationModel();
        var messages = await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == practitionerId && clinicId == clinicId && x.MessageStatusPractitioner != Enums.MessageStatus.Deleted)
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

    /// <summary>
    /// Crude method to set all messages as 'Read'.
    /// </summary>
    public async Task UpdateReadReceiptsClient(int clientId, int pracId)
    {
        await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == pracId && x.MessageStatusClient == Enums.MessageStatus.Unread)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.MessageStatusClient, Enums.MessageStatus.Read));
    }
    
    /// <summary>
    /// Crude method to set all messages as 'Read'.
    /// </summary>
    public async Task UpdateReadReceiptsPractitioner(int clientId, int pracId)
    {
        await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == pracId && x.MessageStatusPractitioner == Enums.MessageStatus.Unread)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.MessageStatusPractitioner, Enums.MessageStatus.Read));
    }
}