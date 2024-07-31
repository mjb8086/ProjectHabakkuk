using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.Clinic;
using HBKPlatform.Models.View.MyND.RoomReservation;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

public class RoomReservationService(IRoomReservationRepository _roomResRepo, IUserService _userService, ITimeslotRepository _timeslotRepo, IConfigurationService _config, ICacheService _cache, 
    IAppointmentRepository _appointmentRepo, IAvailabilityRepository _avaRepo, ITimeslotService _tsSrv, IDateTimeWrapper _dateTime): IRoomReservationService
{
    private List<RoomReservationDto> _reservations;
    private List<TimeslotAvailabilityDto> _weeklyAvaLookup;
    private Dictionary<int, TimeslotAvailabilityDto> _indefAvaLookup;
    
    public async Task Create(int roomId, int weekNum, int timeslotId)
    {
        var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
        await ClashCheck(roomId, timeslotId, weekNum);
        var room = _cache.GetRoom(roomId);
        await _roomResRepo.Create(new RoomReservationDto() { RoomId = roomId, PractitionerId = practitionerId, WeekNum = weekNum, StartTick = timeslotId, ClinicId = room.ClinicId });
    }


    public async Task CancelAsPractitioner(int reservationId)
    {
        await _roomResRepo.UpdateStatusPractitioner(reservationId, Enums.ReservationStatus.CancelledByPractitioner);
    }

    public async Task ApproveReservation(int id, string? note)
    {
        var res = await _roomResRepo.GetReservationAnyTenancy(id);
        if (res.Status == Enums.ReservationStatus.Approved) return;
        await ClashCheck(res.RoomId, res.StartTick, res.WeekNum);
        await _roomResRepo.UpdateStatusClinic(id, Enums.ReservationStatus.Approved, note);
    }
    
    public async Task DenyReservation(int id, string? note)
    {
        var res = await _roomResRepo.GetReservationAnyTenancy(id);
        if (res.Status == Enums.ReservationStatus.Denied) return;
        if (res.Status == Enums.ReservationStatus.Booked) throw new InvalidUserOperationException("Cannot deny a reservation with an appointment booked");
        await _roomResRepo.UpdateStatusClinic(id, Enums.ReservationStatus.Denied, note);
    }
    
    public async Task CancelAsClinic(int id)
    {
        var res = await _roomResRepo.GetReservationAnyTenancy(id);
        if (res.Status == Enums.ReservationStatus.CancelledByClinic) return;
        if (res.Status == Enums.ReservationStatus.Booked) throw new InvalidUserOperationException("Cannot cancel a reservation with an appointment booked");
        await _roomResRepo.UpdateStatusClinic(id, Enums.ReservationStatus.CancelledByClinic);
    }
    
    public async Task ConfirmRoomBookingPractitioner(int id)
    {
        var res = await _roomResRepo.GetReservationAnyTenancy(id);
        // TODO check practitioner owns this reservation
        await _roomResRepo.UpdateStatusPractitioner(id, Enums.ReservationStatus.Booked);
    }

    public async Task<ConfirmReservation> GetConfirmReservationView(int roomId, int weekNum, int timeslotId)
    {
//        var ts = await _timeslotRepo.GetTimeslot(timeslotId);
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
 //       var dateTime = DateTimeHelper.FromTimeslot(dbStartDate, ts, weekNum);
        var room = _cache.GetRoom(roomId);
        
        return new ConfirmReservation()
        {
  //          ProspectiveDateTime = DateTimeHelper.GetFriendlyDateTimeString(dateTime),
            RoomDetails = room.Title,
            RoomId = room.Id,
            WeekNum = weekNum,
            TimeslotId = timeslotId
        };
    }

    public async Task<TimeslotSelect> GetTimeslotSelectView(int roomId)
    {
        var room = _cache.GetRoom(roomId);
        
        // Get availability for the weeks ahead
        var timeslots = await _tsSrv.GetPopulatedFutureTimeslots();
        var tsList = await FilterOutUnsuitableTimeslots(timeslots, roomId);
        
        return new TimeslotSelect()
        {
            AvailableTimeslots = tsList,
            RoomId = roomId,
            RoomTitle = room.Title
        };
    }

    public async Task<MyReservations> GetUpcomingReservationsPractitioner(DateTime now)
    {
        var model = new MyReservations();
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var thisWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        var currentTick = TimeblockHelper.GetCurrentTick(now);
        
        _reservations = await _roomResRepo.GetUpcomingReservationsPractitioner(pracId, thisWeekNum, currentTick);

        model.Requested = BuildRoomResList(Enums.ReservationStatus.Requested, dbStartDate);
        model.Approved = BuildRoomResList(Enums.ReservationStatus.Approved, dbStartDate);
        model.Denied = BuildRoomResList(Enums.ReservationStatus.Denied, dbStartDate);
        model.Cancelled = BuildRoomResList(Enums.ReservationStatus.CancelledByClinic, dbStartDate);
        model.Cancelled.AddRange(BuildRoomResList(Enums.ReservationStatus.CancelledByPractitioner, dbStartDate));

        return model;
    }
    
    public async Task<List<RoomReservationLite>> GetHeldReservationsPractitioner(DateTime? now)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        now ??= DateTime.UtcNow;
        var thisWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now.Value);
        
        _reservations = await _roomResRepo.GetUpcomingReservationsPractitioner(pracId, thisWeekNum, TimeblockHelper.GetCurrentTick(now));

        return BuildRoomResList(Enums.ReservationStatus.Approved, dbStartDate);
    }
    
    public async Task<RoomReservationOverview> GetUpcomingReservationsClinic()
    {
        var model = new RoomReservationOverview();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var now = DateTime.UtcNow;
        var thisWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        
        _reservations = await _roomResRepo.GetUpcomingReservationsClinic(clinicId, thisWeekNum, TimeblockHelper.GetCurrentTick(now));

        model.Requested = BuildRoomResList(Enums.ReservationStatus.Requested, dbStartDate);
        model.Approved = BuildRoomResList(Enums.ReservationStatus.Approved, dbStartDate);
        model.Denied = BuildRoomResList(Enums.ReservationStatus.Denied, dbStartDate);
        model.Cancelled = BuildRoomResList(Enums.ReservationStatus.CancelledByClinic, dbStartDate);
        model.Cancelled.AddRange(BuildRoomResList(Enums.ReservationStatus.CancelledByPractitioner, dbStartDate));

        return model;
    }

    /// <summary>
    /// Get room details for a reservation
    /// </summary>
    public async Task<RoomLite> GetRoomDetailsFromReservation(int roomResId)
    {
        var res = await _roomResRepo.GetReservation(roomResId);
        var roomDetails = _cache.GetRoom(res.RoomId);
        return new RoomLite() { Id = roomDetails.Id, Title = roomDetails.Title };
    }

    public async Task<RoomReservationDto> GetReservation(int reservationId)
    {
        return await _roomResRepo.GetReservation(reservationId);
    }
    
    
    ////////////////////////////////////////////////////////////////////////////////
    // HELPERS
    ////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Build a list of RoomReservationLites of the specified Reservation status.
    /// Requires _reservations and _tsDict to be populated.
    /// </summary>
    private List<RoomReservationLite> BuildRoomResList(Enums.ReservationStatus status, string dbStartDate)
    {
        var list = new List<RoomReservationLite>();
        
        foreach (var res in _reservations)
        {
            if (res.Status != status) continue;
            list.Add(new RoomReservationLite()
            {
                Id = res.Id,
                RoomTitle = _cache.GetRoom(res.RoomId).Title,
                When = DateTimeHelper.GetIsoDateTimeString(
                    DateTimeHelper.FromTick(dbStartDate, res.StartTick, res.WeekNum)),
                Status = res.Status,
                Whom = _cache.GetPractitionerName(res.PractitionerId)
            });
        }
        
        return list;
    }
    
   ////////////////////////////////////////////////////////////////////////////////
   // PRIVATE METHODS
   ////////////////////////////////////////////////////////////////////////////////
   
   /// <summary>
   /// Check existing reservations, appointments and availability to ensure the room is not approved in a double
   /// booking.
   /// </summary>
    private async Task ClashCheck(int roomId, int timeslotId, int weekNum)
    {
        // first, check if there is a reservation already made for the time
        if(await _roomResRepo.CheckForClashingReservationAnyTenant(weekNum, timeslotId, roomId))
        {
            throw new DoubleBookingException("A reservation already exists on this date and time. Cannot continue.");
        } 
        
        // then check whether an appointment exists at this time
        if (true)//await _appointmentRepo.CheckForDoubleBookingsAnyTenant(weekNum, timeslotId, roomId))
        {
            throw new DoubleBookingException("An appointment already exists in the room on this date and time. Cannot continue.");
        }
        
        // finally check the room availability
        if (!await _avaRepo.IsRoomAvailableForWeekAnyTenancy(roomId, weekNum, timeslotId))
        {
            throw new TimeslotUnavailableException("The room is not available at this time.");
        }
    }
   
   /// <summary>
   /// Check existing reservations, appointments and availability to ensure the room is not approved in a double
   /// booking. Exclude the current room res Id from consideration - a double booking cannot happen against itself.
   /// This method checks Appointments on the room res + timeslot across all tenancies.
   /// </summary>
    public async Task VerifyRoomReservationPractitioner(RoomReservationDto roomRes, AppointmentRequestDto appointmentReq)
    {
        // ensure this is actually allowed
        if (roomRes.WeekNum != appointmentReq.WeekNum || !(appointmentReq.StartTick < roomRes.EndTick && appointmentReq.EndTick > roomRes.StartTick))
        {
            throw new InvalidUserOperationException(
                "Cannot book a room outside of its reservation's date and time.");
        } 
        if (roomRes.Status != Enums.ReservationStatus.Approved)
        {
            throw new InvalidUserOperationException($"Room reservation is not approved. Status: {roomRes.Status}");
        }
        
        // now check if there is a reservation already made for the time
        if (await _roomResRepo.CheckForDoubleBookingAnyTenant(roomRes))
        {
            throw new DoubleBookingException("Another reservation already exists on this date and time. Cannot continue.");
        } 
        
        // then check whether an appointment exists at this time across other tenancies
        if (await _appointmentRepo.CheckForDoubleBookingsAnyTenant(appointmentReq, roomRes.RoomId, roomRes.Id))
        {
            throw new DoubleBookingException("An appointment already exists in the room on this date and time. Cannot continue.");
        }
        // finally check the room availability, has the clinic set it to be unavailable?
        if (!await _avaRepo.IsRoomAvailableForWeekAnyTenancy(roomRes.RoomId, roomRes.WeekNum, roomRes.StartTick))
        {
            throw new TimeslotUnavailableException("The room is not available at this time.");
        }
    }
   
        /// <summary>
        /// Takes a list of timeslots and returns only those available to book with the specified roomId
        /// </summary>
        private async Task<SortedSet<TimeslotDto>> FilterOutUnsuitableTimeslots(SortedSet<TimeslotDto> timeslots, int roomId)
        {
            var occupiedTimeslots = await _appointmentRepo.GetFutureOccupiedTimeslotsForRoomAnyTenancy(roomId, _dateTime.Now);
            // populate lookups for IsAvailable check
            _weeklyAvaLookup = await _avaRepo.GetRoomLookupForWeeksAnyTenancy(roomId, timeslots.Select(x => x.WeekNum).Distinct().ToArray());
            _indefAvaLookup = await _avaRepo.GetRoomLookupForIndefAnyTenancy(roomId);
        
            // If the ts is unavailable, return true. Fixme: may be inefficient?
            return new SortedSet<TimeslotDto>(timeslots.Where(x => x.IsNotClashAny(occupiedTimeslots) && IsAvailable(x.WeekNum, x.TimeslotIdx)));
        }
        
        /// <summary>
        /// Determine whether the timeslot is available. Check both definite (weekly) and indefinite timeslots.
        /// Assumes these lookups are populated already.
        /// </summary>
        private bool IsAvailable(int weekNum, int tsId)
        {
            // first check definite (weekly) timeslots
            TimeslotAvailabilityDto? weekAva;
            if ((weekAva = _weeklyAvaLookup.FirstOrDefault(y => y.WeekNum == weekNum && y.TimeslotId == tsId)) != null)
            {
                return weekAva.Availability == Enums.TimeslotAvailability.Available;
            }
            // then indefinite
            if (_indefAvaLookup.TryGetValue(tsId, out var ava))
            {
                return ava.Availability == Enums.TimeslotAvailability.Available;
            }
            // else we know it is unavailable
            return false;
        }
    
}