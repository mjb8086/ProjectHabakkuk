using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Client.Controllers;

/// <summary>
/// HBKPlatform Appointment controller.
/// 
/// Author: Mark Brown
/// Authored: 12/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("Client")]
public class AppointmentController(IBookingService _bookingService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _bookingService.GetClientUpcomingAppointmentsView());
    }
    
    [Route("booking/")]
    public IActionResult Booking()
    {
        return View("Booking/Index");
    }
    
    [Route("booking/timeslotselect")]
    public async Task<IActionResult> TimeslotSelect()
    {
        return View("Booking/TimeslotSelect", await _bookingService.GetAvailableTimeslotsForClient());
    }
    
    [Route("booking/requestreceived")]
    public IActionResult RequestReceived()
    {
        return View("Booking/RequestReceived");
    }
}