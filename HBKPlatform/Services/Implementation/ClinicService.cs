using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
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
public class ClinicService(ApplicationDbContext _db, IClinicRepository _clinicRepository, IHttpContextAccessor httpContextAccessor) : IClinicService
{
    public async Task<bool> VerifyClientAndPracClinicMembership(int clientId, int pracId)
    {
        var client = await _db.Clients.FirstAsync(x => x.Id == clientId);
        var prac = await _db.Practitioners.FirstAsync(x => x.Id == pracId);
        return client.ClinicId == prac.ClinicId;
    }

    public async Task<MyNDInboxModel> GetInboxModel()
    {
        var inboxModel = new MyNDInboxModel();
        inboxModel.ClientDetails = new List<ClientDetailsLite>();
        
        var clinicIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
        if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out int clinicId))
        {
            var clinicDetails = await _clinicRepository.GetCompleteClinic(clinicId);
            foreach (var client in clinicDetails.Clients)
            {
                inboxModel.ClientDetails.Add(new ClientDetailsLite() { Name = $"{client.Forename} {client.Surname}", Id = client.Id });
            }
        }

        return inboxModel;
    }

    public async Task<ClientClinicData> GetClientClinicData()
    {
        var data = new ClientClinicData(); 
        var clinicIdClaim = httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
        if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out int clinicId))
        {
            data.PracId = _db.Practitioners.First(x => x.ClinicId == clinicId).Id;
        }

        return data;
    }

    public async Task<MyNDReceptionModel> GetReceptionModel()
    {
        var data = new MyNDReceptionModel();
        var pracIdClaim = httpContextAccessor.HttpContext.User.FindFirst("PractitionerId");
        if (pracIdClaim != null && int.TryParse(pracIdClaim.Value, out int pracId))
        {
            data.NumUnreadMessages = await _db.ClientMessages.CountAsync(x => x.PractitionerId == pracId && x.MessageOrigin == Enums.MessageOrigin.Client && x.MessageStatusPractitioner == Enums.MessageStatus.Unread);
        }

        return data;
    }
}