using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.Clinic;
using HBKPlatform.Models.View.MyND.RoomReservation;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class RoomReservationService(IRoomReservationRepository _roomResRepo, IUserService _userService, ITimeslotRepository _timeslotRepo, IConfigurationService _config, ICacheService _cache): IRoomReservationService
{
    private List<RoomReservationDto> _reservations;
    private Dictionary<int, TimeslotDto> _tsDict;
    
    public async Task Create(int roomId, int weekNum, int timeslotId)
    {
        var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
        var room = _cache.GetRoom(roomId);
        // todo: clash check here?
        await _roomResRepo.Create(new RoomReservationDto() { RoomId = roomId, PractitionerId = practitionerId, WeekNum = weekNum, TimeslotId = timeslotId, ClinicId = room.ClinicId });
    }

    public async Task CancelAsPractitioner(int reservationId)
    {
        await _roomResRepo.UpdateStatusPractitioner(reservationId, Enums.ReservationStatus.CancelledByPractitioner);
    }
    
    public async Task CancelAsClinic(int reservationId)
    {
        await _roomResRepo.UpdateStatusClinic(reservationId, Enums.ReservationStatus.CancelledByClinic);
    }

    public async Task UpdateStatusClinic(int id, Enums.ReservationStatus status, string? note)
    {
        await _roomResRepo.UpdateStatusClinic(id, status, note);
    }
    
    public async Task UpdateStatusPractitioner(int id, Enums.ReservationStatus status)
    {
        await _roomResRepo.UpdateStatusPractitioner(id, status);
    }

    public async Task<ConfirmReservation> GetConfirmReservationView(int roomId, int weekNum, int timeslotId)
    {
        var ts = await _timeslotRepo.GetTimeslot(timeslotId);
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var dateTime = DateTimeHelper.FromTimeslot(dbStartDate, ts, weekNum);
        var room = _cache.GetRoom(roomId);
        
        return new ConfirmReservation()
        {
            ProspectiveDateTime = DateTimeHelper.GetFriendlyDateTimeString(dateTime),
            RoomDetails = room.Title,
            RoomId = room.Id,
            WeekNum = weekNum,
            TimeslotId = timeslotId
        };
    }

    public async Task<TimeslotSelect> GetTimeslotSelectView(int roomId)
    {
        var allTimeslots = await _timeslotRepo.GetPracticeTimeslots();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var bookingAdvance = int.Parse((await _config.GetSettingOrDefault("BookingAdvanceWeeks")).Value);
        var room = _cache.GetRoom(roomId);
        
        // For now do not regard the availability, just return all timeslots
        return new TimeslotSelect()
        {
            AvailableTimeslots = TimeslotHelper.GetPopulatedFutureTimeslots(DateTime.UtcNow, allTimeslots, dbStartDate, bookingAdvance),
            RoomId = roomId,
            RoomTitle = room.Title
        };
    }

    public async Task<MyReservations> GetUpcomingReservationsPractitioner()
    {
        var model = new MyReservations();
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var thisWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, DateTime.UtcNow);
        
        _tsDict = (await _timeslotRepo.GetPracticeTimeslots()).ToDictionary(x => x.Id);
        _reservations = await _roomResRepo.GetUpcomingReservationsPractitioner(pracId, thisWeekNum);

        model.Requested = BuildRoomResList(Enums.ReservationStatus.Requested, dbStartDate);
        model.Approved = BuildRoomResList(Enums.ReservationStatus.Approved, dbStartDate);
        model.Denied = BuildRoomResList(Enums.ReservationStatus.Denied, dbStartDate);

        return model;
    }
    
    public async Task<RoomReservationOverview> GetUpcomingReservationsClinic()
    {
        var model = new RoomReservationOverview();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var thisWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, DateTime.UtcNow);
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        
        // FIXME: Timeslots are always filtered out because they currently are Practice-tenancy only!
        _tsDict = (await _timeslotRepo.GetPracticeTimeslots()).ToDictionary(x => x.Id);
        _reservations = await _roomResRepo.GetUpcomingReservationsClinic(clinicId, thisWeekNum);

        model.Requested = BuildRoomResList(Enums.ReservationStatus.Requested, dbStartDate);
        model.Approved = BuildRoomResList(Enums.ReservationStatus.Approved, dbStartDate);
        model.Denied = BuildRoomResList(Enums.ReservationStatus.Denied, dbStartDate);

        return model;
    }

    /// <summary>
    /// Build a list of RoomReservationLites of the specified Reservation status.
    /// Requires _reservations and _tsDict to be populated.
    /// </summary>
    private List<RoomReservationLite> BuildRoomResList(Enums.ReservationStatus status, string dbStartDate)
    {
        var list = new List<RoomReservationLite>();
        
        foreach (var res in _reservations)
        {
            if (res.Status != status || !_tsDict.TryGetValue(res.TimeslotId, out var ts)) continue;
            list.Add(new RoomReservationLite()
            {
                Id = res.Id,
                RoomTitle = _cache.GetRoom(res.RoomId).Title,
                When = DateTimeHelper.GetFriendlyDateTimeString(
                    DateTimeHelper.FromTimeslot(dbStartDate, ts, res.WeekNum))
            });
        }
        
        return list;
    }
}