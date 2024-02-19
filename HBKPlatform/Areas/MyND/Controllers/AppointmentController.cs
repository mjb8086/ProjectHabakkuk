using HBKPlatform.Globals;
using HBKPlatform.Models;
using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
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

[Area("MyND"), Authorize(Roles="Practitioner")]
public class AppointmentController(ITreatmentService _treatmentService, IBookingService _bookingService, IAvailabilityManagementService _availabilityMgmt): Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _bookingService.GetMyNDUpcomingAppointmentsView());
    }

    public async Task<IActionResult> TimeslotManagement()
    {
        return View(await _bookingService.GetTimeslotMgmtView());
    }
    
    public async Task<IActionResult> AvailabilityManagement()
    {
        return View(await _availabilityMgmt.GetAvailabilityManagementIndexModel());
    }
    
    public async Task<IActionResult> SetWeekAvailability(int weekNum)
    {
        return View(await _availabilityMgmt.GetAvailabilityModelForWeek(weekNum));
    }
    
    public async Task<IActionResult> SetIndefAvailability()
    {
        return View(await _availabilityMgmt.GetAvailabilityModelForIndef());
    }

    [HttpPost]
    public async Task<IActionResult> DoSetAvailability(int? weekNum, [FromBody] UpdatedAvailability model)
    {
        if (!ModelState.IsValid) throw new Exception("Model bad");
        if (weekNum.HasValue)
        {
            await _availabilityMgmt.UpdateAvailabilityForWeek(weekNum.Value, model);
        }
        else
        {
            await _availabilityMgmt.UpdateAvailabilityForIndef(model);
        }
        return Ok();
    }

    public async Task<IActionResult> DoRevertAvailability(int? weekNum)
    {
        if (weekNum.HasValue)
        {
            await _availabilityMgmt.RevertAvailabilityForWeek(weekNum.Value);
        }
        else
        {
            await _availabilityMgmt.RevertAvailabilityForIndef();
        }

        return Ok();
    }
    
    public async Task<IActionResult> TreatmentManagement()
    {
        return View(await _treatmentService.GetTreatmentMgmtView());
    }
    
    public async Task<IActionResult> CreateTreatment(int? treatmentId)
    {
        return treatmentId.HasValue ? 
            View("TreatmentCreate", await _treatmentService.GetTreatment(treatmentId.Value)) : 
            View("TreatmentCreate", new TreatmentDto() {Requestability = Enums.TreatmentRequestability.ClientAndPrac});
    }

    [HttpPost]
    public async Task<IActionResult> DoCreateTreatment([FromForm] TreatmentDto treatment)
    {
        if (!ModelState.IsValid) throw new Exception("Model bad");
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
        if (!ModelState.IsValid) throw new Exception("Model bad");
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
        if (!ModelState.IsValid) throw new Exception("Model bad");
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
        if (!ModelState.IsValid) throw new Exception("Model bad");
        await _bookingService.DoCancelBooking(appointmentId, model.Reason, Enums.AppointmentStatus.CancelledByPractitioner);
        TempData["Message"] = "Successfully cancelled booking";
        return RedirectToRoute(new { controller = "Appointment", action = "Index" });
    }
    
}