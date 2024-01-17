using System.Data;
using HBKPlatform.Helpers;
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
public class BookingService(ITimeslotRepository _timeslotRepo, IUserService _userService, ICacheService _cacheService, IAppointmentRepository _appointmentRepo, IConfigurationService _config) : IBookingService
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
    private async Task<List<TimeslotDto>> GetTimeslotsForBooking(int clinicId)
    {
        var allTimeslots = await GetAllTimeslots();
        var dbStartDate = (await _config.GetSettingOrDefault("DbStartDate", clinicId)).Value;
        var bookingAdvance = int.Parse((await _config.GetSettingOrDefault("BookingAdvanceWeeks", clinicId)).Value);
        var now = DateTime.UtcNow;
        var currentWeekNum = DateTimeHelper.GetWeekNumFromDateTime(dbStartDate, now);
        var today = DateTimeHelper.ConvertDotNetDay(now.DayOfWeek);
        var nowTime = new TimeOnly(now.Hour, now.Minute, now.Second);

        var futureTs = new List<TimeslotDto>(allTimeslots.Count);

        while (bookingAdvance > 0)
        {
            foreach (var ts in allTimeslots)
            {
                // FIXME: week num is not correct for advance weeks - recalc, clone TS object
                if (ts.Day >= today && ts.Time > nowTime)
                {
                    ts.WeekNum = currentWeekNum;
                    futureTs.Add(ts);
                }
                else
                {
                    ts.WeekNum = currentWeekNum + 1;
                    futureTs.Add(ts);
                }
            }

            bookingAdvance--;
        }

        return futureTs;
    }
    
    public async Task<TimeslotSelectView> GetAvailableTimeslotsClientView(int treatmentId)
    {
        // Do clash checking - only show free and available timeslots in the future
        // Get treatment Id
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var treatments = await _cacheService.GetTreatments(clinicId);
        
        var availableTs =  await GetTimeslotsForBooking(clinicId);
        availableTs = (await ClashCheck(availableTs, 0)).OrderBy(x => x.WeekNum).ThenBy(x => x.Day).ThenBy(x => x.Time).ToList();
        
        if (treatments.TryGetValue(treatmentId, out TreatmentDto treatment))
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
        return timeslots;
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForClient(int clientId)
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        var appointments = await _appointmentRepo.GetAppointmentsForClient(clientId);
        var treatments = await _cacheService.GetTreatments(clinicId);
        var dbStartDate = await _config.GetSettingOrDefault("DbStartDate", clinicId);
        
        foreach (var appointment in appointments)
        {
            var dateTime = DateTimeHelper.FromTimeslot(dbStartDate.Value, appointment.WeekNum, appointment.Timeslot);
            appointment.PractitionerName = _cacheService.GetPracName(appointment.PractitionerId);
            appointment.DateString = dateTime.ToShortDateString();
            appointment.TimeString = dateTime.ToShortTimeString();
            appointment.TreatmentTitle = treatments[appointment.TreatmentId].Title;
        }
        return appointments;
    }
    
    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsForPractitioner(int pracId)
    {
        var clinicId = _userService.GetClaimFromCookie("ClinicId");
        
        var appointments = await _appointmentRepo.GetAppointmentsForPractitioner(pracId);
        var treatments = await _cacheService.GetTreatments(clinicId);
        var dbStartDate = await _config.GetSettingOrDefault("DbStartDate", clinicId);
        
        foreach (var appointment in appointments)
        {
            var dateTime = DateTimeHelper.FromTimeslot(dbStartDate.Value, appointment.WeekNum, appointment.Timeslot);
            appointment.PractitionerName = _cacheService.GetPracName(appointment.PractitionerId);
            appointment.DateString = dateTime.ToShortDateString();
            appointment.TimeString = dateTime.ToShortTimeString();
            appointment.TreatmentTitle = treatments[appointment.TreatmentId].Title;
            appointment.ClientName = _cacheService.GetClientName(appointment.ClientId);
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