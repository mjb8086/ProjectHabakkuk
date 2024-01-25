using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform Clinic service.
/// Middleware for controller and database functionality.
/// 
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
public class ClinicService(ApplicationDbContext _db, ICacheService _cache, IHttpContextAccessor httpContextAccessor) : IClinicService
{
    public async Task<bool> VerifyClientAndPracClinicMembership(int clientId, int pracId)
    {
        var client = await _db.Clients.FirstAsync(x => x.Id == clientId);
        var prac = await _db.Practitioners.FirstAsync(x => x.Id == pracId);
        return client.ClinicId == prac.ClinicId;
    }

    public async Task<InboxModel> GetInboxModel()
    {
        var inboxModel = new InboxModel();
        inboxModel.ClientDetails = new List<ClientDetailsLite>();
        
        var clinicIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
        if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out int clinicId))
        {
            inboxModel.ClientDetails = await _cache.GetClinicClientDetailsLite(clinicId);
        }

        return inboxModel;
    }

    public async Task<ClientClinicData> GetClientClinicData()
    {
        var data = new ClientClinicData(); 
        var clientIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClientId");
        if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int clientId))
        {
            // Currently assumes 1 prac per clinic
            var clientDetailsLite = _cache.GetClientDetailsLite(clientId);
            var pracDetails = await _cache.GetClinicPracDetailsLite(clientDetailsLite.ClinicId);
            data.PracId = pracDetails.First().Id;
            data.MyPracName = pracDetails.First().Name;
            data.NumUnreadMessages = await _db.ClientMessages.CountAsync(x => x.PractitionerId == data.PracId && x.ClientId == clientId && x.MessageOrigin == Enums.MessageOrigin.Practitioner && x.MessageStatusClient == Enums.MessageStatus.Unread);
        }

        return data;
    }

    public async Task<ReceptionModel> GetReceptionModel()
    {
        var data = new ReceptionModel();
        var pracIdClaim = httpContextAccessor.HttpContext.User.FindFirst("PractitionerId");
        if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out int pracId))
        {
            data.NumUnreadMessages = await _db.ClientMessages.CountAsync(x => x.PractitionerId == pracId && x.MessageOrigin == Enums.MessageOrigin.Client && x.MessageStatusPractitioner == Enums.MessageStatus.Unread);
        }

        return data;
    }
}