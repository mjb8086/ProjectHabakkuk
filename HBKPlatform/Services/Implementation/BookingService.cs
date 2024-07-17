using System.Collections.Immutable;
using System.Globalization;
using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
using HBKPlatform.Models.View.MyND;
using HBKPlatform.Repository;

namespace HBKPlatform.Services.Implementation
{
    /// <summary>
    /// HBKPlatform Booking service.
    /// Here for appointment booking and timeslots.
    /// 
    /// Author: Mark Brown
    /// Authored: 11/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class BookingService(IUserService _userService, ICacheService _cacheService, 
        IAppointmentRepository _appointmentRepo, IConfigurationService _config, IDateTimeWrapper _dateTime, IAvailabilityRepository _avaRepo,
        IRoomReservationService _roomRes, ILogger<BookingService> _logger) : IBookingService
    {
        private List<TimeslotAvailabilityDto> _weeklyAvaLookup;
        private Dictionary<int, TimeslotAvailabilityDto> _indefAvaLookup;
        public const int APPOINTMENTS_SELECT_LIMIT = 10;    // currently not used because we just do one fetch
    
        public async Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId)
        {
            // Do clash checking - only show free and available timeslots in the future
            // Get treatment Id
            var practiceId = _userService.GetClaimFromCookie("PracticeId");
            var treatments = await _cacheService.GetTreatments();
            var practitionerId = _cacheService.GetLeadPractitionerId(practiceId);
        
//            var availableTs = (await FilterOutUnsuitableTimeslots(availableTsSet, practitionerId)).OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
        
            if (treatments.TryGetValue(treatmentId, out var treatment) && treatment.Requestability == Enums.TreatmentRequestability.ClientAndPrac) // security filter
            {
                return new TimeslotSelectView()
                {
 //                   AvailableTimeslots = availableTs,
                    TreatmentName = treatment.Title,
                    TreatmentId = treatment.Id
                };
            }

            throw new IdxNotFoundException($"Treatment ID {treatmentId} does not exist or cannot be booked.");
        }

        public async Task<List<TimeblockDto>> GetAvailableTimeblocksPractitioner()
        {
            var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
        }

        /// <summary>
        /// Takes a list of timeslots and returns only those available to book with the specified practitioner
        /// MAY NEED FIXED
        /// </summary>
        private async Task<List<TimeslotDto>> FilterOutUnsuitableTimeslots(SortedSet<TimeslotDto> timeslots, int pracId)
        {
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
//            var futureAppts = await _appointmentRepo.GetFutureAppointmentsForPractitioner(pracId, _dateTime.Now, dbStartDate);
            // populate lookups for IsAvailable check
            _weeklyAvaLookup = await _avaRepo.GetPractitionerLookupForWeeks(pracId, timeslots.Select(x => x.WeekNum).Distinct().ToArray());
            _indefAvaLookup = await _avaRepo.GetPractitionerLookupForIndef(pracId);
        
            // TODO: If new appointment statuses are added, update predicate
 //           var occupiedTimeslots = futureAppts.Where(x => x.Status == Enums.AppointmentStatus.Live).Select(x => x.Timeslot).ToList();
        
            // If the ts is unavailable, return true
           // return timeslots.Where(x => x.IsNotClashAny(occupiedTimeslots) && IsAvailable(x.WeekNum, x.TimeslotIdx)).ToList();

           return new List<TimeslotDto>();
        }


        /// <summary>
        /// Will check that
        /// a) the reservation weeknum and timeslot matches that of the appointment
        /// b) no other appointments exist on any tenancy at the same weeknum and ts using this room
        /// c) the room is still set as available by the clinic on this timeslot && weeknum
        /// </summary>
        private async Task VerifyRoomReservation(RoomReservationDto roomRes, int timeslotId, int weekNum)
        {
            // Check for clashes against other room bookings and appointments.

            // ensure the room is set as available by the clinic
        } 
        

        public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId)
        {
            var appointments = await _appointmentRepo.GetAppointmentsForClient(clientId);
            var treatments = await _cacheService.GetTreatments();
            var dbStartDate = await _config.GetSettingOrDefault("DbStartDate");
        
            foreach (var appointment in appointments)
            {
                var dateTime = DateTimeHelper.FromTick(dbStartDate.Value, appointment.StartTick, appointment.WeekNum);
                appointment.PractitionerName = _cacheService.GetPractitionerName(appointment.PractitionerId);
                appointment.DateString = dateTime.ToShortDateString();
                appointment.TimeString = dateTime.ToShortTimeString();
                appointment.TreatmentTitle = treatments[appointment.TreatmentId].Title;
                appointment.RoomDetails = appointment.RoomId.HasValue
                    ? _cacheService.GetRoom(appointment.RoomId.Value).Title
                    : null;
            }
            return appointments;
        }
    
        public async Task<List<AppointmentLite>> GetUpcomingAppointmentsForPractitioner(int pracId, int currentWeekNum, int currentTick, bool liveOnly)
        {
            var appointments = await _appointmentRepo.GetFutureAppointmentsForPractitioner(pracId, currentWeekNum, currentTick, liveOnly);
            var treatments = await _cacheService.GetTreatments();
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            
            var appointmentsLite = new List<AppointmentLite>();
            foreach (var appointment in appointments)
            {
                var dateTime = DateTimeHelper.FromTick(dbStartDate, appointment.StartTick, appointment.WeekNum);
                appointmentsLite.Add(new AppointmentLite()
                {
                    DateTime = dateTime.ToString("s"),
                    TreatmentTitle = treatments[appointment.TreatmentId].Title,
                    ClientName = _cacheService.GetClientName(appointment.ClientId),
                    Status = appointment.Status,
                    RoomDetails = appointment.RoomId.HasValue ? _cacheService.GetRoom(appointment.RoomId.Value).Title : null,
                    WeekNum = appointment.WeekNum
                });
            }
            return appointmentsLite;
        }

    
        // TO DEPRECATE
        public async Task<ClientUpcomingAppointmentsView> GetClientUpcomingAppointmentsView()
        {
            return new ClientUpcomingAppointmentsView()
            {
                SelfBookingEnabled = await _config.IsSettingEnabled("SelfBookingEnabled"),
                UpcomingAppointments =
                    await GetUpcomingAppointmentsForClient(_userService.GetClaimFromCookie("ClientId"))
            };
        }

        public async Task<BookingConfirm> GetBookingConfirmModel(PractitionerBookingFormModel model)
        {
            var tsWeekNum = model.ParseTsWeekNum();
            return await GetBookingConfirmModel(model.TreatmentId, tsWeekNum[0], tsWeekNum[1], model.RoomResId, model.ClientId);
        }
    
        /// <summary>
        /// Get booking confirm model. Used by both Client and Practitioner views.
        /// In client view, clientId and roomResId are null.
        /// DEPRECATED
        /// </summary>
        public async Task<BookingConfirm> GetBookingConfirmModel(int treatmentId, int timeslotId, int weekNum, int? roomResId, int? clientId)
        {
            var practiceId = _userService.GetClaimFromCookie("PracticeId");
            var pracId = _userService.GetClaimFromCookie("PractitionerId");
            pracId = pracId < 1 ? _cacheService.GetLeadPractitionerId(practiceId) : pracId;
            var treatments = await _cacheService.GetTreatments();
        
            if (!treatments.TryGetValue(treatmentId, out var treatment) || (!clientId.HasValue && treatment.Requestability == Enums.TreatmentRequestability.PracOnly))
            {
                throw new IdxNotFoundException($"TreatmentId {treatmentId} does not exist or cannot be booked.");
            }
        
//            var timeslotDto = await _timeslotRepo.GetTimeslot(timeslotId);
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var model = new BookingConfirm();

            if (roomResId.HasValue && roomResId.Value > 0)
            {
                model.RoomReservationId = roomResId;
                model.RoomReservationDetails = (await _roomRes.GetRoomDetailsFromReservation(roomResId.Value)).Title;
            }
        
            model.TreatmentId = treatmentId;
            model.WeekNum = weekNum;
            model.TimeslotId = timeslotId;
            model.RoomReservationId = roomResId;
            model.PractitionerName = _cacheService.GetPractitionerName(pracId);
            model.ClientId = clientId;
            model.ClientName = clientId.HasValue ? _cacheService.GetClientName(clientId.Value) : "";
            model.TreatmentTitle = treatment.Title;
//            model.BookingDate = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslotDto, weekNum));
            return model;
        }

        public async Task<BookingConfirm> DoBookingClient(AppointmentRequestDto appointmentReq)
        {
            if (!(await _config.IsSettingEnabled("SelfBookingEnabled")))
            {
                _logger.LogWarning("Client attempted booking but self booking has been disabled.");
                throw new InvalidUserOperationException("Cannot create a booking as a client. This feature has been disabled.");
            }
            
            var practiceId = _userService.GetClaimFromCookie("PracticeId");
            var clientId = _userService.GetClaimFromCookie("ClientId");
            var pracId = _cacheService.GetLeadPractitionerId(practiceId); // TODO: Change to prac select on UI
            
            appointmentReq.PractitionerId = pracId;
            appointmentReq.ClientId = clientId;

            return await DoBooking(appointmentReq, true);
        }
    

        public async Task<BookClientTreatment> GetBookClientTreatmentView()
        {
            var practitionerId = _userService.GetClaimFromCookie("PractitionerId");
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var bookingAdvanceWeeks = await _config.GetIntValueOrDefault("BookingAdvanceWeeks");
            
            var now = DateTime.UtcNow;
            var currentTick = TimeslotHelper.GetCurrentTick(now);
            var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        
            var treatments = await _cacheService.GetTreatments();
            var timeblocks = await _appointmentRepo.GetFutureAppointmentTimeblocks(practitionerId, weekNum, weekNum + bookingAdvanceWeeks, currentTick);
            timeblocks = timeblocks.FlattenTimeblocks(false);
//            var timeslots = await _tsSrv.GetPopulatedFutureTimeslots();
 //           var tsList = (await FilterOutUnsuitableTimeslots(timeslots, practitionerId)).OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
 
        
            return new BookClientTreatment()
            {
                Clients = await _cacheService.GetPracticeClientDetailsLite(),
  //              Timeslots = DtoHelpers.ConvertTimeslotsToLite(dbStartDate, tsList),
                Treatments = DtoHelpers.ConvertTreatmentsToLite(treatments.Values),
                HeldReservations = await _roomRes.GetHeldReservationsPractitioner()
            };
        }

        // DEPRECATED
        public async Task<BookingCancel> GetBookingCancelView(int appointmentId)
        {
            var appointment = await _appointmentRepo.GetAppointment(appointmentId);
 //           var timeslot = await _timeslotRepo.GetTimeslot(appointment.StartTick);
            var treatments = await _cacheService.GetTreatments();
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;

            return new BookingCancel()
            {
                AppointmentId = appointment.Id,
                PractitionerName = _cacheService.GetPractitionerName(appointment.PractitionerId),
                ClientName = _cacheService.GetClientName(appointment.ClientId),
//                DateString = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslot, appointment.WeekNum)),
                TreatmentTitle = treatments[appointment.TreatmentId].Title
            };
        }

        public async Task DoCancelBooking(int appointmentId, string reason, Enums.AppointmentStatus actioner)
        {
            // todo - security checks
            await _appointmentRepo.CancelAppointment(appointmentId, reason, actioner);
        }
        
        // New Methods
        public async Task<BookingConfirm> DoBookingPractitioner(AppointmentRequestDto appointmentReq)
        {
            appointmentReq.PractitionerId = _userService.GetClaimFromCookie("PractitionerId");
            return await DoBooking(appointmentReq, false);
        }
        
        /// <summary>
        /// DoBooking method, used by either the Client or the Practitioner.
        /// </summary>
        /// <param name="appointmentReq"></param>
        /// <returns></returns>
        /// <exception cref="IdxNotFoundException"></exception>
        /// <exception cref="InvalidUserOperationException"></exception>
        private async Task<BookingConfirm> DoBooking(AppointmentRequestDto appointmentReq, bool isClientAction)
        {
            var appt = new AppointmentDto();
            RoomLite? roomDetails = null;
            bool hasRoomRes = appointmentReq.RoomResId.HasValue && appointmentReq.RoomResId.Value > 0;
            
            await ClashCheckNew(appointmentReq);
        
            if (hasRoomRes)
            {
                var roomRes = await _roomRes.GetReservation(appointmentReq.RoomResId.Value);
                await _roomRes.VerifyRoomReservationPractitioner(roomRes, appointmentReq);
                appt.RoomId = roomRes.RoomId;
                appt.RoomReservationId = roomRes.Id;
            }

            var treatments = await _cacheService.GetTreatments();
            if (!treatments.TryGetValue(appointmentReq.TreatmentId, out var treatment) || (isClientAction && treatment.Requestability == Enums.TreatmentRequestability.PracOnly))
            {
                throw new IdxNotFoundException($"Treatment does not exist or you do not have permission to book it.");
            }
        
            // all clear? then create the booking
            // TODO: Make this resilient - may involve consolidating more to the repository !! P R I O R I T Y !!
            try
            {
                appt.ClientId = appointmentReq.ClientId;
                appt.PractitionerId = appointmentReq.PractitionerId;
                appt.TreatmentId = appointmentReq.TreatmentId;
                appt.WeekNum = appointmentReq.WeekNum;
                appt.StartTick = appointmentReq.StartTick;
                appt.EndTick = appointmentReq.EndTick;
                
                if (hasRoomRes)
                {
                    await _roomRes.ConfirmRoomBookingPractitioner(appointmentReq.RoomResId.Value);
                }
                await _appointmentRepo.CreateAppointment(appt);
            }
            catch (Exception e)
            {
                throw new InvalidUserOperationException($"Problem when creating booking: {e.Message}.");
            }

            // todo: notify, email
            return new BookingConfirm() // Replace with appt DTO if necessary, or simple success message?
            {
                PractitionerName = _cacheService.GetPractitionerName(appointmentReq.PractitionerId),
                ClientName = _cacheService.GetClientName(appointmentReq.ClientId),
                TreatmentTitle = treatment.Title,
                RoomReservationDetails = roomDetails?.Title ?? "",
                RoomReservationId = appointmentReq.RoomResId,
//                BookingDate = DateTimeHelper.GetFriendlyDateTimeString(DateTimeHelper.FromTimeslot(dbStartDate, timeslotDto, weekNum)) // todo: fix or use frontend with Ts+startTick
            };
        }
        
        /// <summary>
        /// Check that the Appointment repository has no live appointments with timeslots overlapping the prospective
        /// appointment, then check that the timeslot is available for the current practitioner.
        /// </summary>
        /// <param name="appointmentReq"></param>
        /// <exception cref="DoubleBookingException"></exception>
        /// <exception cref="TimeslotUnavailableException"></exception>
        private async Task ClashCheckNew(AppointmentRequestDto appointmentReq)
        {
            // check whether an appointment exists at this time
            if (await _appointmentRepo.CheckForDoubleBooking(appointmentReq.PractitionerId, appointmentReq.WeekNum,
                    appointmentReq.StartTick, appointmentReq.EndTick))
            {
                throw new DoubleBookingException("An appointment already exists at this date and time.");
            }
            
            // finally check the room availability
            if (!await _avaRepo.IsTimeslotAvailable(appointmentReq.WeekNum, appointmentReq.StartTick, 
                    appointmentReq.EndTick, appointmentReq.PractitionerId))
            {
                throw new TimeslotUnavailableException("This timeslot is not available.");
            }
        }

    }
}
