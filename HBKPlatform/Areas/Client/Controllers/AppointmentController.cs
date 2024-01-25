using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Client.Controllers;

/// <summary>
/// HBKPlatform Client appointment controller.
/// 
/// Author: Mark Brown
/// Authored: 12/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("Client")]
public class AppointmentController(IBookingService _bookingService, ITreatmentService _treatmentService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _bookingService.GetClientUpcomingAppointmentsView());
    }
    
    [Route("booking/")]
    public async Task<IActionResult> Booking()
    {
        return View("Booking/Index", await _treatmentService.GetTreatmentsViewForClient());
    }
    
    [Route("booking/timeslotselect")]
    public async Task<IActionResult> TimeslotSelect(int treatmentId)
    {
        return View("Booking/TimeslotSelect", await _bookingService.GetAvailableTimeslotsClientView(treatmentId));
    }
    
    [Route("booking/bookingconfirm")]
    public async Task<IActionResult> BookingConfirm(int treatmentId,  int timeslotId, int weekNum)
    {
        return View("Booking/BookingConfirm", await _bookingService.GetBookingConfirmModel(treatmentId, timeslotId, weekNum));
    }
    
    [Route("booking/bookingconfirmed")]
    public async Task<IActionResult> BookingConfirmed(int treatmentId, int timeslotId, int weekNum)
    {
        var model = await _bookingService.DoBookingClient(treatmentId, timeslotId, weekNum);
        return View("Booking/BookingConfirmed", model);
    }
    
    [Route("booking/cancel")]
    public async Task<IActionResult> CancelBooking(int appointmentId)
    {
        return View("Booking/CancelBooking", await _bookingService.GetBookingCancelView(appointmentId));
    }

    [Route("booking/docancel")]
    public async Task<IActionResult> DoCancelBooking([FromQuery] int appointmentId, [FromForm] CancelAppointmentFormModel model)
    {
        if (!ModelState.IsValid) throw new Exception("Model bad");
        await _bookingService.DoCancelBooking(appointmentId, model.Reason, Enums.AppointmentStatus.CancelledByClient);
        ViewBag.Message = "Successfully cancelled booking";
        return RedirectToRoute(new { controller = "Appointment", action = "Index" });
    }
}