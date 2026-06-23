using Hbk.Platform.Models;
using Hbk.Platform.Models.View.MyND;
using Hbk.Platform.Models.DTO;
using Hbk.Platform.Models.View;
using Hbk.Platform.Models.View.MCP;

namespace Hbk.Platform.Services
{
    public interface IPracticeService
    {
        public Task<bool> VerifyClientPractitionerMembership(int clientId, int practitionerId);
        public Task<InboxModel> GetInboxModel();
        public Task<ClientPracticeData> GetClientReceptionData();
        public Task<ReceptionModel> GetPractitionerReceptionModel();
    }
}