using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View;
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
public class BookingService(ITimeslotRepository _timeslotRepo, IUserService _userService, ICacheService _cacheService, IAppointmentRepository _appointmentRepo) : IBookingService
{
    public async Task<List<TimeslotDto>> GetAllTimeslots()
    {
        return await _timeslotRepo.GetClinicTimeslots(_userService.GetClaimFromCookie("ClinicId"));
    }
    
    public async Task<TimeslotManagementView> GetTimeslotMgmtView()
    {
        return new TimeslotManagementView() { Timeslots = await GetAllTimeslots() };
    }
    
    public async Task<TimeslotDto> GetTimeslot(int timeslotId)
    {
        return await _timeslotRepo.GetTimeslot(timeslotId);
    }
    
    public async Task<TimeslotSelectView> GetAvailableTimeslotsForClient()
    {
        return new TimeslotSelectView() { AvailableTimeslots = await GetAllTimeslots() };
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId)
    {
        var appointments = await _appointmentRepo.GetAppointmentsForClient(clientId);
        foreach (var appointment in appointments)
        {
            // TODO: Get others from cache
            appointment.PractitionerName = _cacheService.GetPracName(appointment.PractitionerId);
            appointment.DateString = "TODO";
            appointment.TimeString = "TODO";
            appointment.TreatmentTitle = "TODO";
        }
        return appointments;
    }
    
    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForPractitioner(int pracId)
    {
        var appointments = await _appointmentRepo.GetAppointmentsForPractitioner(pracId);
        foreach (var appointment in appointments)
        {
            appointment.ClientName = _cacheService.GetClientName(appointment.ClientId);
            appointment.DateString = "TODO";
            appointment.TimeString = "TODO";
            appointment.TreatmentTitle = "TODO";
        }
        return appointments;
    }

    public async Task<MyNDUpcomingAppointmentsView> GetMyNDUpcomingAppointmentsView()
    {
        return new MyNDUpcomingAppointmentsView()
        {
            UpcomingAppointments =
                await GetUpcomingAppointmentsForPractitioner(_userService.GetClaimFromCookie("PractitionerId"))
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
}