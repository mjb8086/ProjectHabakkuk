using HBKPlatform.Models.View;

namespace HBKPlatform.Services;

public interface IAppointmentService
{
    public Task<TreatmentManagementView> GetTreatmentMgmtView();
}