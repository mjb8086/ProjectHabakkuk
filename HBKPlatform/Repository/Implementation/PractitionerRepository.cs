using HBKPlatform.Database;
using HBKPlatform.Helpers;
using HBKPlatform.Models.View;

namespace HBKPlatform.Repository.Implementation
{
    public class PractitionerRepository(ApplicationDbContext applicationDbContext) : IPractitionerRepository
    {
        public Practitioner GetPractitioner(int mciIdx)
        {
            var practitioner =  applicationDbContext.Practitioners.First(x => x.Id.Equals(mciIdx));
            return practitioner;
        }

        public PractitionerViewModel GetPractitionerView(Practitioner practitioner)
        {
            var newPracViewModel = PractitionerHelper.DbModelToViewModel(practitioner);
            // TEMP
            return newPracViewModel;
        }
    }
}

