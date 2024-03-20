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
                TreatmentId = appointmentDto.TreatmentId
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
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status,
                    PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId, Timeslot = new TimeslotDto()
                    {
                        Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration
                    }
                }).AsNoTracking().ToListAsync();
        }
    
        public async Task<List<AppointmentDto>> GetAppointmentsForPractitioner(int pracId)
        {
            return await _db.Appointments.Include("Timeslot").Where(x => x.PractitionerId == pracId)
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new AppointmentDto()
                {
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status,
                    PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId, TimeslotId = x.TimeslotId, Timeslot = new TimeslotDto()
                    {
                        Day = x.Timeslot.Day, Time = x.Timeslot.Time, Duration = x.Timeslot.Duration
                    }
                }).AsNoTracking().ToListAsync();
        }
    
        /// <summary>
        /// Get upcoming appointments for the practitionerId.
        /// </summary>
        public async Task<List<AppointmentDto>> GetFutureAppointmentsForPractitioner(int pracId, DateTime now)
        {
            var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate")).Value;
            var weekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
            var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        
            return await _db.Appointments.Include("Timeslot")
                .Where(x => x.Status == Enums.AppointmentStatus.Live && (x.WeekNum > weekNum || x.WeekNum == weekNum && x.Timeslot.Day > today || x.WeekNum == weekNum && x.Timeslot.Day == today && x.Timeslot.Time >= TimeOnly.FromDateTime(now)))
                .OrderBy(x => x.WeekNum).ThenBy(x => x.Timeslot.Day).ThenBy(x => x.Timeslot.Time)
                .Select(x => new AppointmentDto()
                {
                    Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, Note = x.Note, Status = x.Status,
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

    }
}
