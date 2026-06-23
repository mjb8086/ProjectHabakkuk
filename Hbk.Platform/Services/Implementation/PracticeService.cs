using Hbk.Models;
using Hbk.Models.View.MyND;
using Hbk.Platform.Repository;
using Hbk.Platform.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hbk.Platform.Services.Implementation
{
    /// <summary>
    /// Hbk.Platform Practice service.
    /// Serves up controller and database functionality.
    /// 
    /// Author: Mark Brown
    /// Authored: 13/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class PracticeService(ICacheService _cache, IUserService _userSrv, IClientMessageRepository _messageRepo, IUserRepository _userRepo, IConfigurationService _config) : IPracticeService
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
                data.SelfBookingEnabled = await _config.IsSettingEnabled("SelfBookingEnabled");
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