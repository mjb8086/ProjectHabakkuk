using HBKPlatform.Globals;
using HBKPlatform.Models.API.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform reception service.
/// 
/// Author: Mark Brown
/// Authored: 10/05/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

public class ReceptionService(IBookingService _bookingService, IUserService _userService, IConfigurationService _config, 
    IAppointmentRepository _appointmentRepo, IRecordRepository _recordRepo, IClientRepository _clientRepo) : IReceptionService
{
    public async Task<ReceptionSummaryData> GetReceptionSummaryData()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var now = DateTime.UtcNow;
        
        var appts = await _bookingService.GetUpcomingAppointmentsForPractitioner(pracId);
        var model = new ReceptionSummaryData()
        {
            UpcomingAppointments = appts.Where(x => x.Status == Enums.AppointmentStatus.Live).ToList(),
            RecentCancellations =
                appts.Where(x => x.Status is Enums.AppointmentStatus.CancelledByClient or Enums.AppointmentStatus.
                    CancelledByPractitioner).ToList(),
            NumAppointmentsCompleted = await _appointmentRepo.GetNumberOfCompletedAppointments(pracId, dbStartDate, now),
            PriorityItems = await _recordRepo.GetClientRecordsLite(pracId, true),
            NumClientsRegistered = _clientRepo.GetClientCount()
        };
        
        return model;
    }
}