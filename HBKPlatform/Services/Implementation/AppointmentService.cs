using HBKPlatform.Models.View;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class AppointmentService(IUserService _userService, ITreatmentRepository _treatmentRepo) : IAppointmentService
{
    public async Task<TreatmentManagementView> GetTreatmentMgmtView()
    {
        return new TreatmentManagementView()
        {
            Treatments = await _treatmentRepo.GetClinicTreatments(_userService.GetClaimFromCookie("ClinicId"))
        };
    }
}