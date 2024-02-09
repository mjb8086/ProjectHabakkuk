using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// Client messaging service.
/// Middleware for controller and database functionality.
/// 
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class ClientMessagingService(IHttpContextAccessor _httpContextAccessor, IClientMessageRepository _clientMessageRepository, 
     IClinicService _clinicService, ICacheService _cache) : IClientMessagingService
{
     public async Task SendMessage(string messageBody, int recipientId)
     {
          if (string.IsNullOrWhiteSpace(messageBody)) return;
          
          var clientIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("ClientId");
          var pracIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("LeadPractitionerId");
          var clinicIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
          int clientId, pracId, clinicId;
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
               throw new InvalidOperationException("Message data is incomplete.");
          }

          if (!(clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out clinicId)))
          {
               throw new InvalidOperationException("Clinic ID is required.");
          }

          // Check that the users are part of the same clinic
          if (!(await _clinicService.VerifyClientAndPracClinicMembership(clientId, pracId)))
          {
               throw new InvalidOperationException("Client and practitioner are not members of the same clinic.");
          }

          // if that's okay, clean it and then save it.
          messageBody = messageBody.Trim();
          await _clientMessageRepository.SaveMessage(pracId, clientId, clinicId, messageBody, messageOrigin);
          
          // TODO: Send email and update total unread
     }

     // TODO: merge these into one method like above
     public async Task<ClientMessageConversationModel> GetConversationClient(int pracId, int max)
     {
          // Get logged in user and find his clientId 
          var clientIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("ClientId");
          // TODO: Security and access checks
          if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int clientId))
          {
               var clientDetailsLite = _cache.GetClientDetailsLite(clientId);
               var model = await _clientMessageRepository.GetConversation(pracId, clientId, clientDetailsLite.ClinicId, max);
               
               model.PractitionerId = pracId;
               model.CurrentConverser = Enums.MessageOrigin.Client;
               model.Recipient = _cache.GetPracName(pracId);
               model.Sender = clientDetailsLite.Name;
               await _clientMessageRepository.UpdateReadReceiptsClient(clientId, pracId);
               return model;
          }
          return null;
     }
     
     public async Task<ClientMessageConversationModel> GetConversationPractitioner(int clientId, int max)
     {
          // Get logged in user and find his pracId ... TODO: Make this uniform or have a user cache!
          var pracIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("LeadPractitionerId");
          
          // TODO: Security and access checks
          if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out int pracId))
          {
               var pracDetailsLite = _cache.GetPracDetailsLite(pracId);
               var model = await _clientMessageRepository.GetConversation(pracId, clientId, pracDetailsLite.ClinicId, max);
               model.ClientId = clientId;
               model.CurrentConverser = Enums.MessageOrigin.Practitioner;
               model.Sender = pracDetailsLite.Name;
               model.Recipient = _cache.GetClientName(clientId);
               await _clientMessageRepository.UpdateReadReceiptsPractitioner(clientId, pracId);
               return model;
          }
          return null;
     }
}