using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services
{
    public interface IPracticeService
    {
        public Task<bool> VerifyClientPractitionerMembership(int clientId, int practitionerId);
        public Task<InboxModel> GetInboxModel();
        public Task<ClientPracticeData> GetClientPracticeData();
        public Task<ReceptionModel> GetReceptionModel();
    }
}