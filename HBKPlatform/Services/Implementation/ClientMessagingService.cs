using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;

namespace HBKPlatform.Services.Implementation;

public class ClientMessagingService(IHttpContextAccessor httpContextAccessor, IClientMessageRepository _clientMessageRepository, IClinicService _clinicService) : IClientMessagingService
{
     public async Task SendMessage(string messageBody, int recipientId)
     {
          var clientIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClientId");
          var pracIdClaim = httpContextAccessor.HttpContext.User.FindFirst("PractitionerId");
          var clinicIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
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

          // if that's okay, save it.
          await _clientMessageRepository.SaveMessage(pracId, clientId, clinicId, messageBody, messageOrigin);
     }

     public async Task<ClientMessageConversationModel> GetConversationClient(int pracId, int max)
     {
          // Get logged in user and find his clientId 
          var clientIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClientId");
          if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int clientId))
          {
               // TODO: Security and access checks
               return await _clientMessageRepository.GetConversation(pracId, clientId, max);
          }
          return null;
     }
     
     public async Task<ClientMessageConversationModel> GetConversationPractitioner(int clientId, int max)
     {
          // Get logged in user and find his pracId 
          var pracIdClaim = httpContextAccessor.HttpContext.User.FindFirst("PractitionerId");
          if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out int pracId))
          {
               // TODO: Security and access checks
               return await _clientMessageRepository.GetConversation(pracId, clientId, max);
          }
          return null;
     }
}