using HBKPlatform.Database;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IPractitionerRepository
    {
        public Practitioner GetPractitioner(int mciIdx);
        public Task<List<PracDetailsLite>> GetClinicPracs(int clinicId);
    }
}

