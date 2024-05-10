using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Helpers;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    /// <summary>
    /// HBKPlatform Appointment repository.
    /// 
    /// Author: Mark Brown
    /// Authored: 12/01/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    public class AppointmentRepository(ApplicationDbContext _db, IConfigurationService _config) : IAppointmentRepository
    {
        public async Task CreateAppointment(AppointmentDto appointmentDto)
        {
            var appointment = new Appointment()
            {
                ClientId = appointmentDto.ClientId,
                PractitionerId = appointmentDto.PractitionerId,
                TimeslotId = appointmentDto.TimeslotId,
                WeekNum = appointmentDto.WeekNum,
                TreatmentId = appointmentDto.TreatmentId,
                RoomId = appointmentDto.RoomId,
                RoomReservationId = appointmentDto.RoomReservationId
            };
            await _db.AddAsync(appointment);
            await _db.SaveChangesAsync();
        }

        public async Task<AppointmentDto> GetAppointment(int appointmentId)
        {
            return await _db.Appointments.Where(x => x.Id == appointmentId).Select(x => new AppointmentDto()
            {
                Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId
            }).AsNoTracking().FirstOrDefaultAsync() ?? throw new IdxNotFoundException($"Appointment Id {appointmentId} not found.");
        }

        public async Task<List<AppointmentDto>> GetAppointmentsForClient(int clientId)
        {
            return await _db.Appointments.Include("Timeslot").Where(x => x.ClientId == clientId && x.Status == Enums.AppointmentStatus.Live)
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new AppointmentDto()
                {
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status, RoomId = x.RoomId,
                    PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId, Timeslot = new TimeslotDto()
                    {
                        Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration
                    }
                }).AsNoTracking().ToListAsync();
        }
    
        /// <summary>
        /// Get all appointments for the Practitioner, optionally filtered by status.
        /// </summary>
        public async Task<List<AppointmentDto>> GetAppointmentsForPractitioner(int pracId, Enums.AppointmentStatus? status)
        {
            var query = _db.Appointments.Include("Timeslot");
            if (status.HasValue) query = query.Where(x => x.Status == status.Value);
            
            return await query.Where(x => x.PractitionerId == pracId)
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new AppointmentDto()
                {
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status, RoomId = x.RoomId,
                    PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId, Timeslot = new TimeslotDto()
                    {
                        Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration
                    }
                }).AsNoTracking().ToListAsync();
        }
        
        /// <summary>
        /// Get upcoming appointments for the practitionerId.
        /// TODO: Return only timeslots for booking clash checking
        /// </summary>
        public async Task<List<AppointmentDto>> GetFutureAppointmentsForPractitioner(int pracId, DateTime now, bool liveOnly)
        {
            var query = _db.Appointments.Include("Timeslot");
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
            var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);

            if (liveOnly) query = query.Where(x => x.Status == Enums.AppointmentStatus.Live);
        
            return await query 
                .Where(x => x.PractitionerId == pracId && (x.WeekNum > weekNum || x.WeekNum == weekNum && x.Timeslot.Day > today || x.WeekNum == weekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now)))
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new AppointmentDto()
                {
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status, RoomId = x.RoomId,
                    PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId, Timeslot = new TimeslotDto()
                    {
                        Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration, WeekNum = x.WeekNum
                    }
                }).AsNoTracking().ToListAsync();
        }

        public async Task CancelAppointment(int appointmentId, string reason, Enums.AppointmentStatus cancelActioner)
        {
            // is appointment already cancelled?
            var appointment = await _db.Appointments.Include("Timeslot").FirstOrDefaultAsync(x => x.Id == appointmentId) ??
                              throw new IdxNotFoundException($"Appointment ID {appointmentId} does not exist.");

            if (appointment.Status != Enums.AppointmentStatus.Live)
                throw new InvalidUserOperationException("Appointment is already cancelled.");
        
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            if (DateTimeHelper.FromTimeslot(dbStartDate, TimeslotDto.FromDbTimeslot(appointment.Timeslot), appointment.WeekNum) < DateTime.UtcNow)
                throw new InvalidUserOperationException("Cannot cancel appointments in the past.");

            // if not, update appointment to be cancelled and append reason
            appointment.Status = cancelActioner;
            appointment.CancellationReason = reason;
            await _db.SaveChangesAsync();
        }

        
        /// <summary>
        /// Check all tenancies for a matching booking for the room on the WeekNum and TimeslotId. If there is a 'current'
        /// room reservation Id, then this is not a clash, so it is excluded from consideration. (i.e. used by the
        /// practitioner when creating an appointment with the reservation)
        /// </summary>
        public async Task<bool> CheckForDoubleBookingsAnyTenant(int weekNum, int timeslotId, int roomId, int? currentRoomResId)
        {
            if (currentRoomResId.HasValue)
            {
                return await _db.Appointments.IgnoreQueryFilters().AnyAsync(x =>
                    x.WeekNum == weekNum && x.TimeslotId == timeslotId && x.RoomId == roomId && x.RoomReservationId != currentRoomResId);
            }
            return await _db.Appointments.IgnoreQueryFilters().AnyAsync(x =>
                x.WeekNum == weekNum && x.TimeslotId == timeslotId && x.RoomId == roomId);
        }
        
        /// <summary>
        /// Get future and occupied timeslots for the roomId. Ignores query filters because rooms and other bookings will belong to
        /// other tenancies.
        /// </summary>
        public async Task<List<TimeslotDto>> GetFutureOccupiedTimeslotsForRoomAnyTenancy(int roomId, DateTime now)
        {
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
            var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        
            return await _db.Appointments.Include("Timeslot").IgnoreQueryFilters()
                .Where(x => x.RoomId == roomId && x.Status == Enums.AppointmentStatus.Live && (x.WeekNum > weekNum || x.WeekNum == weekNum && x.Timeslot.Day > today || x.WeekNum == weekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now)))
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new TimeslotDto()
                {
                    Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration, WeekNum = x.WeekNum
                }).AsNoTracking().ToListAsync();
        }
        
        
        /// <summary>
        /// Statistics: Get the count of past appointments that have been completed by the Practitioner.
        /// </summary>
        public async Task<int> GetNumberOfCompletedAppointments(int pracId, string dbStartDate, DateTime now)
        {
            var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
            var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
            
            return await _db.Appointments.Include("Timeslot")
                .Where(x => x.PractitionerId == pracId && x.Status == Enums.AppointmentStatus.Live &&
                            (x.WeekNum < weekNum || x.WeekNum == weekNum && x.Timeslot.Day < today ||
                             x.WeekNum == weekNum && x.Timeslot.Day == today &&
                             x.Timeslot.Time < TimeOnly.FromDateTime(now))).CountAsync();
        }
        

    }
}
