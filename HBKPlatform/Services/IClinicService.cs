using HBKPlatform.Models.View;

namespace HBKPlatform.Services;

public interface IClinicService
{
    public Task<bool> VerifyClientAndPracClinicMembership(int clientId, int pracId);
    public Task<MyNDInboxModel> GetInboxModel();
}