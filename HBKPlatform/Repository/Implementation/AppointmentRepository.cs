using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

/// <summary>
/// HBKPlatform Appointment repository.
/// 
/// Author: Mark Brown
/// Authored: 12/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
public class AppointmentRepository(ApplicationDbContext _db) : IAppointmentRepository
{
    public async Task CreateAppointment(AppointmentDto appointmentDto)
    {
        var appointment = new Appointment()
        {
            ClientId = appointmentDto.ClientId,
            ClinicId = appointmentDto.ClinicId,
            PractitionerId = appointmentDto.PractitionerId,
            TimeslotId = appointmentDto.TimeslotId,
            WeekNum = appointmentDto.WeekNum
        };
        await _db.AddAsync(appointment);
        await _db.SaveChangesAsync();
    }

    public async Task<AppointmentDto> GetAppointment(int appointmentId)
    {
        return await _db.Appointments.Where(x => x.Id == appointmentId).Select(x => new AppointmentDto()
        {
            Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, ClinicId = x.ClinicId, Note = x.Note, PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId
        }).AsNoTracking().FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Appointment Id {appointmentId} not found.");
    }

    public async Task<List<AppointmentDto>> GetAppointmentsForClient(int clientId)
    {
        // TODO: order by timeslot day and time
        return await _db.Appointments.Where(x => x.ClientId == clientId).OrderBy(x => x.WeekNum).Select(x => new AppointmentDto()
        {
            Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, ClinicId = x.ClinicId, Note = x.Note,
            PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId
        }).AsNoTracking().ToListAsync();
    }
    
    public async Task<List<AppointmentDto>> GetAppointmentsForPractitioner(int pracId)
    {
        // TODO: order by timeslot day and time
        return await _db.Appointments.Where(x => x.PractitionerId == pracId).OrderBy(x => x.WeekNum).Select(x => new AppointmentDto()
        {
            Id = x.Id, WeekNum = x.WeekNum, ClientId = x.ClientId, ClinicId = x.ClinicId, Note = x.Note,
            PractitionerId = x.PractitionerId, TreatmentId = x.TreatmentId
        }).AsNoTracking().ToListAsync();
    }
    
}