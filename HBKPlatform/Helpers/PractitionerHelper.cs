/******************************
* HBK Practitioner Helper
* Helper methods for practitioner classes
*
* Author: Mark Brown
* Authored: 24/09/2022
******************************/

using HBKPlatform.Database;
using HBKPlatform.Models.View;

namespace HBKPlatform.Helpers
{
    public class PractitionerHelper
    {
        public static PractitionerViewModel DbModelToViewModel(Practitioner dbPrac)
        {
            return new PractitionerViewModel()
            {
                Idx = dbPrac.Id,
                Name = dbPrac.Forename + dbPrac.Surname,
                DOB = dbPrac.DateOfBirth,
                Title = dbPrac.Title,
                Location = dbPrac.Location,
                Bio = dbPrac.Bio,
                Img = dbPrac.Img
            };
        }


        public static List<PractitionerViewModel> DbListToViewList(List<Practitioner> dbPracs)
        {
            var viewModels = new List<PractitionerViewModel>();

            foreach(var dbPrac in dbPracs)
            {
                viewModels.Add(DbModelToViewModel(dbPrac));
            }

            return viewModels;
        }
    }
}

