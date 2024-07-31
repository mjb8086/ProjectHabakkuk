using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.API.MyND;
using HBKPlatform.Models.DTO;
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
    IAppointmentRepository _appointmentRepo, IClientRecordService _recordService, IClientRepository _clientRepo, 
    IRoomReservationService _roomResService, IClientMessagingService _clientMessagingSrv) : IReceptionService
{
    public async Task<ReceptionSummaryData> GetReceptionSummaryData()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        
        var now = DateTime.UtcNow;
        var currentTick = TimeblockHelper.GetCurrentTick(now);
        var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        
        var appts = await _bookingService.GetUpcomingAppointmentsForPractitioner(pracId, weekNum, currentTick, false);
        
        // TODO: Cache stats like num appts completed, num clients registered.
        var model = new ReceptionSummaryData()
        {
            UpcomingAppointments = appts.Where(x => x.Status == Enums.AppointmentStatus.Live).Take(BookingService.APPOINTMENTS_SELECT_LIMIT).ToList(),
            RecentCancellations = appts.Where(x => x.Status is Enums.AppointmentStatus.CancelledByClient or Enums.AppointmentStatus.
                    CancelledByPractitioner).Take(BookingService.APPOINTMENTS_SELECT_LIMIT).ToList(),
            NumAppointmentsCompleted = await _appointmentRepo.GetNumberOfCompletedAppointments(weekNum, currentTick, pracId),
            PriorityItems = await _recordService.GetPopulatedLiteRecords(true),
            RoomReservations = await _roomResService.GetHeldReservationsPractitioner(now),
            NumClientsRegistered = _clientRepo.GetClientCount(),
        };
        
        model.AdditionalUpcoming = model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT > 0
            ? model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT
            : 0;
        model.AdditionalCancellations = model.RecentCancellations.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT > 0
            ? model.UpcomingAppointments.Count() - BookingService.APPOINTMENTS_SELECT_LIMIT
            : 0;
        model.UnreadMessageDetails = await _clientMessagingSrv.GetUnreadMessageDetailsAsPractitioner(pracId);
        model.WeeklyAppointmentsChartData = appts.Where(x => x.Status == Enums.AppointmentStatus.Live)
            .GroupBy(x => x.WeekNum)
            .OrderBy(x => x.Key)
            .Select(g => new ChartDatapoint() { x = DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, g.Key, DateTimeHelper.FRIENDLY_DAY_FORMAT_NO_YEAR), y = g.Count().ToString() })
            .ToList();
        
        return model;
    }
}