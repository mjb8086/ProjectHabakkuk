using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform Appointment service.
/// Middleware for user controller and database functionality. Covers Appointments, treatments, and Timeslots.
/// 
/// Author: Mark Brown
/// Authored: 10/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class AppointmentService(IUserService _userService, ITreatmentRepository _treatmentRepo) : IAppointmentService
{
    public async Task<TreatmentManagementView> GetTreatmentMgmtView()
    {
        return new TreatmentManagementView()
        {
            Treatments = await _treatmentRepo.GetClinicTreatments(_userService.GetClaimFromCookie("ClinicId"))
        };
    }

    public async Task CreateTreatment(TreatmentDto treatment)
    {
        treatment.ClinicId = _userService.GetClaimFromCookie("ClinicId");
        await _treatmentRepo.CreateTreatment(treatment);
    }

    public async Task DeleteTreatment(int treatmentId)
    {
        // todo: confirm user is member of clinic
        await _treatmentRepo.Delete(treatmentId);
    }

    public async Task UpdateTreatment(TreatmentDto treatment)
    {
        await _treatmentRepo.UpdateTreatment(treatment);
    }

    public async Task<TreatmentDto> GetTreatment(int treatmentId)
    {
        return await _treatmentRepo.GetTreatment(treatmentId);
    }

    public async Task<ClientTreatmentSelectView> GetTreatmentsViewForClient()
    {
        return new ClientTreatmentSelectView()
        {
            Treatments = await _treatmentRepo.GetClinicTreatments(_userService.GetClaimFromCookie("ClinicId"), true)
        };
    }
    
}