using System;
using HBKPlatform.Database;
using HBKPlatform.Models.View;

namespace HBKPlatform.Repository
{
    public interface IPractitionerRepository
    {
        public Practitioner GetPractitioner(int mciIdx);
    }
}

