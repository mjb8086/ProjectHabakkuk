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
public class AppointmentController(IAppointmentService _appointmentService): Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult TimeslotManagement()
    {
        return View();
    }
    
    public IActionResult AvailabilityManagement()
    {
        return View();
    }
    
    public async Task<IActionResult> TreatmentManagement()
    {
        return View(await _appointmentService.GetTreatmentMgmtView());
    }
    
    public async Task<IActionResult> CreateTreatment()
    {
        return View("TreatmentCreate");
    }
}