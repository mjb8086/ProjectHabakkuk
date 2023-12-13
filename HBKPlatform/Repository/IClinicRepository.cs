using HBKPlatform.Database;

namespace HBKPlatform.Repository;

public interface IClinicRepository
{
    public Task<Clinic> GetClinicAlone(int clinicIdx);
    public Task<Clinic> GetCompleteClinic(int clinicIdx);
}