using HBKPlatform.Database;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Services.Implementation
{
    /// <summary>
    /// HBKPlatform Practice service.
    /// Serves up controller and database functionality.
    /// 
    /// Author: Mark Brown
    /// Authored: 13/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class PracticeService(ApplicationDbContext _db, ICacheService _cache, IUserService _userSrv) : IPracticeService
    {
        public async Task<bool> VerifyClientPractitionerMembership(int clientId, int practitionerId)
        {
            // TODO: Deprecate or move to repository.
            var client = await _db.Clients.FirstAsync(x => x.Id == clientId);
            var prac = await _db.Practitioners.FirstAsync(x => x.Id == practitionerId);
            return client.PracticeId == prac.PracticeId && client.TenancyId == prac.TenancyId;
        }

        public async Task<InboxModel> GetInboxModel()
        {
            return new () { ClientDetails = await _cache.GetPracticeClientDetailsLite() };
        }

        public async Task<ClientPracticeData> GetClientPracticeData()
        {
            var clientId = _userSrv.GetClaimFromCookie("ClientId");
            var leadPracId = _cache.GetLeadPractitionerId(_userSrv.GetClaimFromCookie("PracticeId"));
            var practitionerDetailsLite = await _cache.GetPracticePractitionerDetailsLite();
            
            var data = new ClientPracticeData(); 
            {
                data.PracId = practitionerDetailsLite[leadPracId].Id;
                data.MyPracName = practitionerDetailsLite[leadPracId].Name;
                // TODO: Move to repository.
                data.NumUnreadMessages = await _db.ClientMessages.CountAsync(x => x.PractitionerId == data.PracId && x.ClientId == clientId && x.MessageOrigin == Enums.MessageOrigin.Practitioner && x.MessageStatusClient == Enums.MessageStatus.Unread);
            }

            return data;
        }

        public async Task<ReceptionModel> GetReceptionModel()
        {
            var practitionerId = _userSrv.GetClaimFromCookie("PractitionerId");
            var data = new ReceptionModel();
            {
                data.NumUnreadMessages = await _db.ClientMessages.CountAsync(x => x.PractitionerId == practitionerId && x.MessageOrigin == Enums.MessageOrigin.Client && x.MessageStatusPractitioner == Enums.MessageStatus.Unread);
            }

            return data;
        }


    }
}