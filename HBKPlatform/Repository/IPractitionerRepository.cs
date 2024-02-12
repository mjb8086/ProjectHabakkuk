using HBKPlatform.Database;
using HBKPlatform.Models.DTO;

namespace HBKPlatform.Repository
{
    public interface IPractitionerRepository
    {
        public Practitioner GetPractitioner(int mciIdx);
        public Task<List<PracDetailsUac>> GetClinicPracs(int clinicId);
        public Task<Dictionary<int, bool>> GetPracLockoutStatusDict(int clinicId);
    }
}

