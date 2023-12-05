using HBKPlatform.Database;
using HBKPlatform.Helpers;
using HBKPlatform.Models.View;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    public class PractitionerRepository(HbkContext hbkContext) : IPractitionerRepository
    {
        public Practitioner GetPractitioner(int mciIdx)
        {
            var practitioner =  hbkContext.Practitioner.First(x => x.Idx.Equals(mciIdx));
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

