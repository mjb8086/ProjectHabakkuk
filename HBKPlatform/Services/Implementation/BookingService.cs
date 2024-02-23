using System.Data;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation;

/// <summary>
/// HBKPlatform Booking service.
/// Middleware for appointment booking and timeslots.
/// 
/// Author: Mark Brown
/// Authored: 11/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class BookingService(ITimeslotRepository _timeslotRepo, IUserService _userService, ICacheService _cacheService, 
    IAppointmentRepository _appointmentRepo, IConfigurationService _config, IDateTimeWrapper _dateTime, IAvailabilityRepository _avaRepo) : IBookingService
{
    private List<TimeslotAvailabilityDto> _weeklyAvaLookup;
    private Dictionary<int, TimeslotAvailabilityDto> _indefAvaLookup;
    
    private async Task<List<TimeslotDto>> GetTimeslotsForBooking()
    {
        var allTimeslots = await _timeslotRepo.GetClinicTimeslots();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var bookingAdvance = int.Parse((await _config.GetSettingOrDefault("BookingAdvanceWeeks")).Value);
        var now = _dateTime.Now;
        var thisWeek = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        var nowTime = new TimeOnly(now.Hour, now.Minute, now.Second);

        var futureTs = new List<TimeslotDto>(allTimeslots.Count);

        var maxWeek = thisWeek + bookingAdvance;
        var currentWeekNum = thisWeek;
        while (currentWeekNum < maxWeek)
        {
            foreach (var ts in allTimeslots)
            {
                var newTs = ts.Clone();
                // split the timeslots at NOW, half will be this week, the preceding will be 'shifted' to the final week
                if (currentWeekNum == thisWeek && (ts.Day < today || ts.Day == today && ts.Time < nowTime))
                {
                    newTs.WeekNum = maxWeek;
                }
                else 
                {
                    newTs.WeekNum = currentWeekNum;
                } 
                newTs.Description = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, newTs));
                futureTs.Add(newTs);
            }

            currentWeekNum++;
        }

        return futureTs;
    }
    
    public async Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId)
    {
        // Do clash checking - only show free and available timeslots in the future
        // Get treatment Id
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var treatments = await _cacheService.GetTreatments();
        var pracId = _cacheService.GetLeadPracId(clinicId);
        
        var availableTs =  await GetTimeslotsForBooking();
        availableTs = (await ClashCheck(availableTs, pracId)).OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
        
        if (treatments.TryGetValue(treatmentId, out var treatment))
        {
            return new TimeslotSelectView()
            {
                AvailableTimeslots = availableTs,
                TreatmentName = treatment.Title,
                TreatmentId = treatment.Id
            };
        }

        throw new MissingPrimaryKeyException($"Treatment ID {treatmentId} does not exist");
    }

    private async Task<List<TimeslotDto>> ClashCheck(List<TimeslotDto> timeslots, int pracId)
    {
        var futureAppts = await _appointmentRepo.GetFutureAppointmentsForPractitioner(pracId, DateTime.UtcNow);
        _weeklyAvaLookup = await _avaRepo.GetAvailabilityLookupForWeeks(pracId, timeslots.Select(x => x.WeekNum).Distinct().ToArray());
        _indefAvaLookup = await _avaRepo.GetAvailabilityLookupForIndef(pracId);
        
        // TODO: If new appointment statuses are added, update predicate
        var occupiedTimeslots = futureAppts.Where(x => x.Status == Enums.AppointmentStatus.Live).Select(x => x.Timeslot).ToList();
        
        // If the ts is unavailable, return true
        return timeslots.Where(x => x.IsNotClashAny(occupiedTimeslots) && IsAvailable(x.WeekNum, x.Id)).ToList();
    }

    /// <summary>
    /// Check whether the timeslot is free for the practitioner.
    /// </summary>
    /// <returns>TRUE - timeslot clashes with another appointment, or is set to unavailable.</returns>
    private async Task<bool> ClashCheckSingle(int timeslotId, int pracId, int weekNum)
    {
        var futureAppts = await _appointmentRepo.GetFutureAppointmentsForPractitioner(pracId, DateTime.UtcNow);
        _weeklyAvaLookup = await _avaRepo.GetAvailabilityLookupForWeek(pracId, weekNum);
        _indefAvaLookup = await _avaRepo.GetAvailabilityLookupForIndef(pracId);
        
        // If the ts is unavailable, return true
        if (!IsAvailable(weekNum, timeslotId)) return true;
        
        // TODO: If new appointment statuses are added, update predicate
        var occupiedTimeslots = futureAppts.Where(x => x.Status == Enums.AppointmentStatus.Live).Select(x => x.Timeslot).ToList();
        var ts = await _timeslotRepo.GetTimeslot(timeslotId);
        ts.WeekNum = weekNum;
        
        // check for clashes with other appointments
        foreach (var occupiedTs in occupiedTimeslots)
        {
            if (ts.IsClash(occupiedTs)) return true;
        }
        return false;
    }
    
    /// <summary>
    /// Determine whether the timeslot is available. Check both definite (weekly) and indefinite timeslots.
    /// Assumes these lookups are populated already.
    /// </summary>
    private bool IsAvailable(int weekNum, int tsId)
    {
        // first check definite (weekly) timeslots
        TimeslotAvailabilityDto weekAva;
        if ((weekAva = _weeklyAvaLookup.FirstOrDefault(y => y.WeekNum == weekNum && y.TimeslotId == tsId)) != null)
        {
            return weekAva.Availability == Enums.TimeslotAvailability.Available;
        }
        // then indefinite
        if (_indefAvaLookup.TryGetValue(tsId, out var ava))
        {
            return ava.Availability == Enums.TimeslotAvailability.Available;
        }
        // else we know it is available
        return true;
    }
    

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId)
    {
        var appointments = await _appointmentRepo.GetAppointmentsForClient(clientId);
        var treatments = await _cacheService.GetTreatments();
        var dbStartDate = await _config.GetSettingOrDefault("DbStartDate");
        
        foreach (var appointment in appointments)
        {
            var dateTime = DateTimeHelper.FromTimeslot(dbStartDate.Value, appointment.Timeslot, appointment.WeekNum);
            appointment.PractitionerName = _cacheService.GetPracName(appointment.PractitionerId);
            appointment.DateString = dateTime.ToShortDateString();
            appointment.TimeString = dateTime.ToShortTimeString();
            appointment.TreatmentTitle = treatments[appointment.TreatmentId].Title;
        }
        return appointments;
    }
    
    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForPractitioner(int pracId)
    {
        var appointments = await _appointmentRepo.GetAppointmentsForPractitioner(pracId);
        var treatments = await _cacheService.GetTreatments();
        var dbStartDate = await _config.GetSettingOrDefault("DbStartDate");
        
        foreach (var appointment in appointments)
        {
            var dateTime = DateTimeHelper.FromTimeslot(dbStartDate.Value, appointment.Timeslot, appointment.WeekNum);
            appointment.DateString = dateTime.ToShortDateString();
            appointment.TimeString = dateTime.ToShortTimeString();
            appointment.TreatmentTitle = treatments[appointment.TreatmentId].Title;
            appointment.ClientName = _cacheService.GetClientName(appointment.ClientId);
        }
        return appointments;
    }

    public async Task<UpcomingAppointmentsView> GetMyNDUpcomingAppointmentsView()
    {
        var now = DateTime.UtcNow;
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        
        var appts = await GetUpcomingAppointmentsForPractitioner(_userService.GetClaimFromCookie("PractitionerId"));
        return new UpcomingAppointmentsView()
        {
            UpcomingAppointments = appts.Where(x => x.Status == Enums.AppointmentStatus.Live && (x.WeekNum > weekNum || x.WeekNum == weekNum && x.Timeslot.Day > today || x.WeekNum == weekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now))).ToList(),
            RecentCancellations = appts.Where(x => x.Status is Enums.AppointmentStatus.CancelledByPractitioner or Enums.AppointmentStatus.CancelledByClient).ToList(),
            PastAppointments = appts.Where(x => x.Status == Enums.AppointmentStatus.Live && (x.WeekNum < weekNum || x.WeekNum == weekNum && x.Timeslot.Day < today || x.WeekNum == weekNum && x.Timeslot.Day == today && x.Timeslot.Time < TimeOnly.FromDateTime(now))).ToList()
        };
    }
    
    public async Task<ClientUpcomingAppointmentsView> GetClientUpcomingAppointmentsView()
    {
        return new ClientUpcomingAppointmentsView()
        {
            UpcomingAppointments =
                await GetUpcomingAppointmentsForClient(_userService.GetClaimFromCookie("ClientId"))
        };
    }

    public async Task<BookingConfirm> GetBookingConfirmModel(PracBookingFormModel model)
    {
        var tsWeekNum = model.ParseTsWeekNum();
        return await GetBookingConfirmModel(model.TreatmentId, tsWeekNum[0], tsWeekNum[1], model.ClientId);
    }
    
    /// <summary>
    /// Get booking confirm model. Used by both Client and Practitioner views. In client view, clientId is null.
    /// </summary>
    public async Task<BookingConfirm> GetBookingConfirmModel(int treatmentId, int timeslotId, int weekNum, int? clientId)
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        pracId = pracId < 1 ? _cacheService.GetLeadPracId(clinicId) : pracId;
        var treatments = await _cacheService.GetTreatments();
        var treatmentTitle = treatments.TryGetValue(treatmentId, out var treatment) ? treatment.Title : "";
        var timeslotDto = await _timeslotRepo.GetTimeslot(timeslotId);
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        
        return new BookingConfirm()
        {
            TreatmentId = treatmentId,
            WeekNum = weekNum,
            TimeslotId = timeslotId,
            PracctitionerName = _cacheService.GetPracName(pracId),
            ClientId = clientId,
            ClientName = clientId.HasValue ? _cacheService.GetClientName(clientId.Value) : "",
            TreatmentTitle = treatmentTitle,
            BookingDate = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslotDto, weekNum))
        };
    }

    public async Task<BookingConfirm> DoBookingClient(int treatmentId, int timeslotId, int weekNum)
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var clientId = _userService.GetClaimFromCookie("ClientId");
        var pracId = _cacheService.GetLeadPracId(clinicId);

        return await DoBooking(timeslotId, pracId, weekNum, clientId, treatmentId);
    }

    public async Task<BookingConfirm> DoBookingPractitioner(int treatmentId, int timeslotId, int weekNum, int clientId)
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");

        return await DoBooking(timeslotId, pracId, weekNum, clientId, treatmentId);
    }
    
    private async Task<BookingConfirm> DoBooking(int timeslotId, int pracId, int weekNum, int clientId, int treatmentId)
    {
        // first check for no clashes
        if (await ClashCheckSingle(timeslotId, pracId, weekNum))
        {
            throw new InvalidOperationException("Another appointment has already been booked into the timeslot.");
        }

        // all clear? then create the booking
        try
        {
            await _appointmentRepo.CreateAppointment(new AppointmentDto()
            {
                ClientId = clientId, PractitionerId = pracId, TreatmentId = treatmentId, WeekNum = weekNum,
                TimeslotId = timeslotId
            });
        }
        catch (Exception e)
        {
           // todo: log exact exception
           throw new InvalidOperationException("Problem when creating booking. Please try again.");
        }

        // todo: notify, email
        var treatments = await _cacheService.GetTreatments();
        var treatmentTitle = treatments.TryGetValue(treatmentId, out var treatment) ? treatment.Description : "";
        var timeslotDto = await _timeslotRepo.GetTimeslot(timeslotId);
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        
        return new BookingConfirm()
        {
            TreatmentId = treatmentId,
            WeekNum = weekNum,
            TimeslotId = timeslotId,
            PracctitionerName = _cacheService.GetPracName(pracId),
            ClientName = _cacheService.GetClientName(clientId),
            TreatmentTitle = treatmentTitle,
            BookingDate = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslotDto, weekNum))
        };
    }

    public async Task<BookClientTreatment> GetBookClientTreatmentView()
    {
        var pracId = _userService.GetClaimFromCookie("PractitionerId");
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
        
        var treatments = await _cacheService.GetTreatments();
        var clients = await _cacheService.GetClinicClientDetailsLite();
        var timeslots = await GetTimeslotsForBooking();
        timeslots = (await ClashCheck(timeslots, pracId)).OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
        
        var timeslotsLite = DtoHelpers.ConvertTimeslotsToLite(dbStartDate, timeslots);
        var treatmentsLite = DtoHelpers.ConvertTreatmentsToLite(treatments.Values);

        return new BookClientTreatment()
        {
            Clients = clients,
            Timeslots = timeslotsLite,
            Treatments = treatmentsLite
        };
    }

    public async Task<BookingCancel> GetBookingCancelView(int appointmentId)
    {
        var appointment = await _appointmentRepo.GetAppointment(appointmentId);
        var timeslot = await _timeslotRepo.GetTimeslot(appointment.TimeslotId);
        var treatments = await _cacheService.GetTreatments();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;

        return new BookingCancel()
        {
            AppointmentId = appointment.Id,
            PractitionerName = _cacheService.GetPracName(appointment.PractitionerId),
            ClientName = _cacheService.GetClientName(appointment.ClientId),
            DateString = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslot, appointment.WeekNum)),
            TreatmentTitle = treatments[appointment.TreatmentId].Title
        };
    }

    public async Task DoCancelBooking(int appointmentId, string reason, Enums.AppointmentStatus actioner)
    {
        // todo - security checks
        await _appointmentRepo.CancelAppointment(appointmentId, reason, actioner);
    }

}