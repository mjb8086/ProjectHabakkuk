using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MCP.Controllers;

/// <summary>
/// HBKPlatform MCP Clinic management controller.
/// 
/// Author: Mark Brown
/// Authored: 07/02/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("MCP")]
[Authorize]
public class ClinicManagementController(IClinicService _clinicService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    public async Task<IActionResult> ViewClinic(int clinicId)
    {
        return View(await _clinicService.GetClinicModel(clinicId));
    }

    [HttpPost]
    public async Task<IActionResult> DoUpdateClinic(int clinicId, [FromForm] ClinicDto model)
    {
        model.Id = clinicId;
        if (!ModelState.IsValid) throw new Exception("Model Bad");
        await _clinicService.UpdateClinic(model);
        return RedirectToRoute(new { controller = "ClinicManagement", action = "ViewClinic", clinicId=clinicId });
    }
    
    public async Task<IActionResult> ListClinics()
    {
        return View(await _clinicService.GetListClinicsView());
    }

    public async Task<IActionResult> PasswordReset()
    {
        return View();
    }
    
    public async Task<IActionResult> RegisterClinic()
    {
        return View();
    }

}
