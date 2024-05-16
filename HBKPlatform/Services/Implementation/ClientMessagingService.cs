using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;
using MissingFieldException = System.MissingFieldException;

namespace HBKPlatform.Services.Implementation
{
     /// <summary>
     /// Client messaging service.
     /// 
     /// Author: Mark Brown
     /// Authored: 13/12/2023
     /// 
     /// © 2023 NowDoctor Ltd.
     /// </summary>
     public class ClientMessagingService(IHttpContextAccessor _httpContextAccessor, IClientMessageRepository _clientMessageRepository, 
          IPracticeService practiceService, ICacheService _cache, IUserService _userService) : IClientMessagingService
     {
          public async Task SendMessage(string messageBody, int recipientId)
          {
               if (string.IsNullOrWhiteSpace(messageBody)) return;
          
               var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("ClientId");
               var pracIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("PractitionerId");
               int clientId, pracId;
               Enums.MessageOrigin messageOrigin;

               if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out clientId))
               {
                    pracId = recipientId;
                    messageOrigin = Enums.MessageOrigin.Client;
               }
               else if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out pracId))
               {
                    clientId = recipientId;
                    messageOrigin = Enums.MessageOrigin.Practitioner;
               }
               else
               {
                    throw new MissingFieldException("Message data is incomplete.");
               }

               // Check that the users are part of the same practice
               if (!(await practiceService.VerifyClientPractitionerMembership(clientId, pracId)))
               {
                    throw new InvalidUserOperationException("Client and practitioner are not members of the same practice.");
               }

               // if that's okay, clean it and then save it.
               messageBody = messageBody.Trim();
               await _clientMessageRepository.SaveMessage(pracId, clientId, messageBody, messageOrigin);
          
               // TODO: Send email and update total unread
          }

          // TODO: merge these into one method like above
          public async Task<ClientMessageConversationModel> GetConversationClient(int pracId, int max)
          {
               // Get logged in user and find his clientId 
               var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("ClientId");
               // TODO: Security and access checks - ensure the client belongs to this prac
               if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int clientId))
               {
                    var clientDetailsLite = _cache.GetClientDetailsLite(clientId);
                    var model = await _clientMessageRepository.GetConversation(pracId, clientId, max);
               
                    model.PractitionerId = pracId;
                    model.CurrentConverser = Enums.MessageOrigin.Client;
                    model.Recipient = _cache.GetPractitionerName(pracId);
                    model.Sender = clientDetailsLite.Name;
                    await _clientMessageRepository.UpdateReadReceiptsClient(clientId, pracId);
                    return model;
               }
               return null;
          }
     
          public async Task<ClientMessageConversationModel> GetConversationPractitioner(int clientId, int max)
          {
               // Get logged in user and find his pracId
               var pracIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("PractitionerId");
          
               // TODO: Security and access checks - ensure the client belongs to this prac
               if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out int pracId))
               {
                    var pracDetailsLite = _cache.GetPractitionerDetailsLite(pracId);
                    var model = await _clientMessageRepository.GetConversation(pracId, clientId, max);
                    model.ClientId = clientId;
                    model.CurrentConverser = Enums.MessageOrigin.Practitioner;
                    model.Sender = pracDetailsLite.Name;
                    model.Recipient = _cache.GetClientName(clientId);
                    await _clientMessageRepository.UpdateReadReceiptsPractitioner(clientId, pracId);
                    return model;
               }
               return null;
          }

          public async Task<List<UnreadMessageDetailLite>> GetUnreadMessageDetailsAsPractitioner(int? pracId)
          {
               pracId ??= _userService.GetClaimFromCookie("PractitionerId");
               var unreadDetails = await _clientMessageRepository.GetUnreadMessageDetailsAsPractitioner(pracId.Value);
               // populate client names - todo, move to ui, one lookup???
               foreach (var unread in unreadDetails)
               {
                    unread.Name = _cache.GetClientName(unread.ClientId);
               }
               return unreadDetails;
          }
     }
}
