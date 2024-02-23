using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Models.View.MyND;

namespace HBKPlatform.Services;

public interface IClinicService
{
    public Task<bool> VerifyClientAndPracClinicMembership(int clientId, int pracId);
    public Task<InboxModel> GetInboxModel();
    public Task<ClientClinicData> GetClientClinicData();
    public Task<ReceptionModel> GetReceptionModel();
}