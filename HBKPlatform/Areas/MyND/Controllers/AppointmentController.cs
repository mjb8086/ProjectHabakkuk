using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform Appointment controller.
/// 
/// Author: Mark Brown
/// Authored: 10/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

[Area("MyND")]
public class AppointmentController(IAppointmentService _appointmentService, IBookingService _bookingService): Controller
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
        return View(await _appointmentService.GetTreatmentMgmtView());
    }
    
    public async Task<IActionResult> CreateTreatment(int? treatmentId)
    {
        return treatmentId.HasValue ? 
            View("TreatmentCreate", await _appointmentService.GetTreatment(treatmentId.Value)) : 
            View("TreatmentCreate", new TreatmentDto());
    }

    [HttpPost]
    public async Task<IActionResult> DoCreateTreatment([FromForm] TreatmentDto treatment)
    {
        await _appointmentService.CreateTreatment(treatment);
        return RedirectToRoute(new { controller = "Appointment", action = "TreatmentManagement" });
    }
    
    [HttpGet]
    public async Task<IActionResult> DeleteTreatment(int treatmentId)
    {
        await _appointmentService.DeleteTreatment(treatmentId);
        return RedirectToRoute(new { controller = "Appointment", action = "TreatmentManagement" });
    }
    
    [HttpPut]
    public async Task<IActionResult> DoUpdateTreatment([FromBody] TreatmentDto treatment)
    {
        // todo: Verify user has right to update 
        await _appointmentService.UpdateTreatment(treatment);
        return Ok();
    }
    
}