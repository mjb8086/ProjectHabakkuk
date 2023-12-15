using HBKPlatform.Database;
using HBKPlatform.Helpers;
using HBKPlatform.Models.View;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// Practitioner Repository.
///
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
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

