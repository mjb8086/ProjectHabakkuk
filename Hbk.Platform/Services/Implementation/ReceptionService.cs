using Hbk.Common.Globals;
using Hbk.Common.Helpers;
using Hbk.Models.API.API.MyND;
using Hbk.Models.DTO;
using Hbk.Models.View.MyND;
using Hbk.Platform.Helpers;
using Hbk.Platform.Repository;

namespace Hbk.Platform.Services.Implementation;

/// <summary>
/// Hbk.Platform reception service.
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
    public const int ReceptionItemLimit = 5;

    public Task<ReceptionSummaryData> GetReceptionSummaryData()
    {
        return BuildReceptionSummaryData();
    }

    public async Task<ReceptionModel> GetReceptionModel()
    {
        var summary = await BuildReceptionSummaryData();

        return new ReceptionModel
        {
            NumUnreadMessages = summary.UnreadMessageDetails.Sum(x => x.UnreadMessageCount),
            NumClientsRegistered = summary.NumClientsRegistered,
            NumAppointmentsCompleted = summary.NumAppointmentsCompleted,
            RecentBookings = summary.UpcomingAppointments,
            RecentCancellations = summary.RecentCancellations,
            PriorityItems = summary.PriorityItems
        };
    }

    private async Task<ReceptionSummaryData> BuildReceptionSummaryData()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var now = DateTime.UtcNow;
        
        var appts = await _bookingService.GetUpcomingAppointmentsForPractitioner(pracId, false);
        var upcomingAppointments = appts
            .Where(x => x.Status == Enums.AppointmentStatus.Live)
            .ToList();
        var recentCancellations = appts
            .Where(x => x.Status is Enums.AppointmentStatus.CancelledByClient or Enums.AppointmentStatus.CancelledByPractitioner)
            .ToList();
        var priorityItems = (await _recordService.GetPopulatedLiteRecords(true))
            .OrderByDescending(x => x.Date)
            .Take(ReceptionItemLimit)
            .ToList();
        
        // TODO: Cache stats like num appts completed, num clients registered.
        var model = new ReceptionSummaryData
        {
            UpcomingAppointments = upcomingAppointments.Take(ReceptionItemLimit).ToList(),
            RecentCancellations = recentCancellations.Take(ReceptionItemLimit).ToList(),
            NumAppointmentsCompleted = await _appointmentRepo.GetNumberOfCompletedAppointments(pracId, dbStartDate, now),
            PriorityItems = priorityItems,
            RoomReservations = await _roomResService.GetHeldReservationsPractitioner(),
            NumClientsRegistered = _clientRepo.GetClientCount(),
            AdditionalUpcoming = Math.Max(0, upcomingAppointments.Count - ReceptionItemLimit),
            AdditionalCancellations = Math.Max(0, recentCancellations.Count - ReceptionItemLimit),
            UnreadMessageDetails = await _clientMessagingSrv.GetUnreadMessageDetailsAsPractitioner(pracId),
            WeeklyAppointmentsChartData = upcomingAppointments
                .GroupBy(x => x.WeekNum)
                .OrderBy(x => x.Key)
                .Select(g => new ChartDatapoint
                {
                    x = DateTimeHelper.GetDateRangeStringFromWeekNum(dbStartDate, g.Key, DateTimeHelper.FRIENDLY_DAY_FORMAT_NO_YEAR),
                    y = g.Count().ToString()
                })
                .ToList()
        };
        
        return model;
    }
}
