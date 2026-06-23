using Hbk.Models;
using Hbk.Models.View.MyND;

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