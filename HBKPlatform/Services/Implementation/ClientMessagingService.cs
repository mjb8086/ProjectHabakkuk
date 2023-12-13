using Microsoft.AspNetCore.Identity;

using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;

namespace HBKPlatform.Services.Implementation;

public class ClientMessagingService(IHttpContextAccessor httpContextAccessor, ClientMessageRepository _clientMessageRepository, IClinicRepository _clinicRepository) : IClientMessagingService
{
     public async Task SendMessage(ClientMessage message)
     {
          // Security - check that the current user is the same as the sender in the body

          // Check that the users are part of the same clinic
          
          // Set values for unread messages
          message.DateOpened = null;
          message.MessageStatus = Enums.MessageStatus.Unread;
          
          _clientMessageRepository.SaveMessage(message);
     }

     public async Task<ClientMessageConversationModel> GetConversation(string sender, string recipient, int max)
     {
          // Assume sender and recipient are in the same clinic, so only one conversation
          // TODO: Security and access checks
          return await _clientMessageRepository.GetConversation(sender, recipient, max);
     }
}