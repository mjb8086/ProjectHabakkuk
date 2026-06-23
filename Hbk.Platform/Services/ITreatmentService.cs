using Hbk.Platform.Models.DTO;
using Hbk.Platform.Models.View;

namespace Hbk.Platform.Services
{
    public interface ITreatmentService
    {
        public Task<TreatmentManagementView> GetTreatmentMgmtView();
        public Task<List<TreatmentLite>> GetTreatmentsLite();
        public Task CreateTreatment(TreatmentDto treatment);
        public Task DeleteTreatment(int treatmentId);
        public Task UpdateTreatment(TreatmentDto treatment);
        public Task<TreatmentDto> GetTreatment(int treatmentId);
        public Task<ClientTreatmentSelectView> GetTreatmentsViewForClient();
    }
}