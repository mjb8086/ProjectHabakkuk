using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform MyND Appointment controller.
/// 
/// Author: Mark Brown
/// Authored: 10/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

[Area("MyND")]
public class AppointmentController(ITreatmentService _treatmentService, IBookingService _bookingService): Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _bookingService.GetMyNDUpcomingAppointmentsView());
    }

    public async Task<IActionResult> TimeslotManagement()
    {
        return View(await _bookingService.GetTimeslotMgmtView());
    }
    
    public IActionResult AvailabilityManagement()
    {
        return View();
    }
    
    public async Task<IActionResult> TreatmentManagement()
    {
        return View(await _treatmentService.GetTreatmentMgmtView());
    }
    
    public async Task<IActionResult> CreateTreatment(int? treatmentId)
    {
        return treatmentId.HasValue ? 
            View("TreatmentCreate", await _treatmentService.GetTreatment(treatmentId.Value)) : 
            View("TreatmentCreate", new TreatmentDto());
    }

    [HttpPost]
    public async Task<IActionResult> DoCreateTreatment([FromForm] TreatmentDto treatment)
    {
        await _treatmentService.CreateTreatment(treatment);
        TempData["Message"] = "Successfully created treatment.";
        return RedirectToRoute(new { controller = "Appointment", action = "TreatmentManagement" });
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteTreatment(int treatmentId)
    {
        await _treatmentService.DeleteTreatment(treatmentId);
        return RedirectToRoute(new { controller = "Appointment", action = "TreatmentManagement" });
    }
    
    [HttpPut]
    public async Task<IActionResult> DoUpdateTreatment([FromBody] TreatmentDto treatment)
    {
        // todo: Verify user has right to update 
        await _treatmentService.UpdateTreatment(treatment);
        TempData["Message"] = "Successfully updated treatment.";
        return RedirectToRoute(new { controller = "Appointment", action = "TreatmentManagement" });
    }

    [HttpGet]
    public async Task<IActionResult> BookClientTreatment()
    {
        return View(await _bookingService.GetBookClientTreatmentView());
    }

    [HttpPost]
    public async Task<IActionResult> BookingConfirm([FromForm] PracBookingFormModel appointment)
    {
        return View(await _bookingService.GetBookingConfirmModel(appointment));
    }
    
    public async Task<IActionResult> BookingConfirmed(int treatmentId, int timeslotId, int weekNum, int clientId)
    {
        return View(await _bookingService.DoBookingPractitioner(treatmentId, timeslotId, weekNum, clientId));
    }

    public async Task<IActionResult> CancelBooking(int appointmentId)
    {
        return View(await _bookingService.GetBookingCancelView(appointmentId));
    }

    public async Task<IActionResult> DoCancelBooking([FromQuery] int appointmentId, [FromForm] CancelAppointmentFormModel model)
    {
        await _bookingService.DoCancelBooking(appointmentId, model.Reason, Enums.AppointmentStatus.CancelledByPractitioner);
        TempData["Message"] = "Successfully cancelled booking";
        return RedirectToRoute(new { controller = "Appointment", action = "Index" });
    }
    
}