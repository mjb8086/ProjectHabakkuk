using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// ClientMessage Repository.
/// All messaging between practitioners and clients is stored and fetched here.
///
/// Author: Mark Brown
/// Authored: 13/12/2023
/// </summary>
public class ClientMessageRepository(ApplicationDbContext _db) : IClientMessageRepository
{
    public async Task SaveMessage(int practitionerId, int clientId, int clinicId, string messageBody, Enums.MessageOrigin messageOrigin)
    {
        // PreviousMessageId - null on first message, other fields will have been constructed by the UI.
        var message = new ClientMessage()
        {
            DateOpened = null,
            MessageStatus = Enums.MessageStatus.Unread,
            PreviousMessageId = null,
            PractitionerId = practitionerId,
            ClientId = clientId,
            ClinicId = clinicId,
            MessageOrigin = messageOrigin,
            MessageBody = messageBody
        };

        await _db.AddAsync(message);
    }

    public async Task<ClientMessageConversationModel> GetConversation(int practitionerId, int clientId, int clinicId, int next = 0)
    {
        var convo = new ClientMessageConversationModel();
        var messages = await _db.ClientMessages.Where(x => x.ClientId == clientId && x.PractitionerId == practitionerId && clinicId == clinicId)
            .OrderByDescending(x => x.Id).Take(next).ToListAsync();

        if (messages.Count == 0)
        {
            return convo;
        }

        // TODO: reconsider use of previous and next message Id... may not be needed
        convo.Messages = new List<ClientMessageDto>();
        
        foreach (var message in messages)
        {
            convo.Messages.Add( new ClientMessageDto
            {
                MessageOrigin = message.MessageOrigin,
                MessageBody = message.MessageBody,
                MessageStatus = message.MessageStatus
            });
        }
        
        return convo;
    }
}