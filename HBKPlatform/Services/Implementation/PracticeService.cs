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
    public class PracticeService(ICacheService _cache, IUserService _userSrv, IClientMessageRepository _messageRepo, IUserRepository _userRepo) : IPracticeService
    {
        public async Task<bool> VerifyClientPractitionerMembership(int clientId, int practitionerId)
        {
            return await _userRepo.VerifyClientPractitionerMembership(clientId, practitionerId);

        }

        public async Task<InboxModel> GetInboxModel()
        {
            return new () { ClientDetails = await _cache.GetPracticeClientDetailsLite() };
        }

        public async Task<ClientPracticeData> GetClientReceptionData()
        {
            var clientId = _userSrv.GetClaimFromCookie("ClientId");
            var leadPracId = _cache.GetLeadPractitionerId(_userSrv.GetClaimFromCookie("PracticeId"));
            var practitionerDetailsLite = await _cache.GetPracticePractitionerDetailsLite();
            
            var data = new ClientPracticeData(); 
            {
                data.PracId = practitionerDetailsLite[leadPracId].Id;
                data.MyPracName = practitionerDetailsLite[leadPracId].Name;
                data.NumUnreadMessages = await _messageRepo.GetUnreadMessagesAsClient(leadPracId, clientId);
            }

            return data;
        }

        public async Task<ReceptionModel> GetPractitionerReceptionModel()
        {
            var practitionerId = _userSrv.GetClaimFromCookie("PractitionerId");
            var data = new ReceptionModel();
            {
                data.NumUnreadMessages = await _messageRepo.GetUnreadMessagesAsPractitioner(practitionerId);
            }

            return data;
        }


    }
}