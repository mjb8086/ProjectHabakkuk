using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.API.MyND;
using HBKPlatform.Repository;
using HBKPlatform.Repository.Implementation;

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
    IAppointmentRepository _appointmentRepo, IClientRecordService _recordService, IClientRepository _clientRepo, 
    IRoomReservationService _roomResService, IAppointmentRepository _apptRepo) : IReceptionService
{
    public async Task<ReceptionSummaryData> GetReceptionSummaryData()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var now = DateTime.UtcNow;
        
        var appts = await _bookingService.GetUpcomingAppointmentsForPractitioner(pracId, false);
        
        var model = new ReceptionSummaryData()
        {
            UpcomingAppointments = appts.Where(x => x.Status == Enums.AppointmentStatus.Live).Take(BookingService.APPOINTMENTS_SELECT_LIMIT).ToList(),
            RecentCancellations =
                appts.Where(x => x.Status is Enums.AppointmentStatus.CancelledByClient or Enums.AppointmentStatus.
                    CancelledByPractitioner).Take(BookingService.APPOINTMENTS_SELECT_LIMIT).ToList(),
            NumAppointmentsCompleted = await _appointmentRepo.GetNumberOfCompletedAppointments(pracId, dbStartDate, now),
            PriorityItems = await _recordService.GetPopulatedLiteRecords(true),
            RoomReservations = await _roomResService.GetHeldReservationsPractitioner(),
            NumClientsRegistered = _clientRepo.GetClientCount(),
        };
        
        model.AdditionalUpcoming = model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT > 0
            ? model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT
            : 0;
        model.AdditionalCancellations = model.RecentCancellations.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT > 0
            ? model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT
            : 0;
        
        return model;
    }
}