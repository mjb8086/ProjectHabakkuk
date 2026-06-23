using Hbk.Platform.Models.DTO;

namespace Hbk.Platform.Repository
{
    public interface ITreatmentRepository
    {
        public Task<TreatmentDto> GetTreatment(int treatmentId);
        public Task<List<TreatmentLite>> GetTreatmentsLite(bool clientOnly = false);
        public Task CreateTreatment(TreatmentDto treatmentDto);
        public Task UpdateTreatment(TreatmentDto treatmentDto);
        public Task Delete(int treatmentId);
    }
}