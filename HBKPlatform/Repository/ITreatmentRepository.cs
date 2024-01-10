using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository;

public interface ITreatmentRepository
{
    public Task<TreatmentDto> GetTreatment(int treatmentId);
    public Task<List<TreatmentLite>> GetClinicTreatments(int clinicId);
    public Task CreateTreatment(TreatmentDto treatmentDto);
}