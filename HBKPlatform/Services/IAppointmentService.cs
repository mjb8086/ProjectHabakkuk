using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;

namespace HBKPlatform.Services;

public interface IAppointmentService
{
    public Task<TreatmentManagementView> GetTreatmentMgmtView();
    public Task CreateTreatment(TreatmentDto treatment);
    public Task DeleteTreatment(int treatmentId);
    public Task UpdateTreatment(TreatmentDto treatment);
    public Task<TreatmentDto> GetTreatment(int treatmentId);
    public Task<ClientTreatmentSelectView> GetTreatmentsViewForClient();
}