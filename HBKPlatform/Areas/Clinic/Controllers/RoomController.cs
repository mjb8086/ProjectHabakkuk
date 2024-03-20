using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Clinic.Controllers;

/// <summary>
/// HBKPlatform Clinic Room Controller.
/// 
/// Author: Mark Brown
/// Authored: 20/03/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("Clinic"), Authorize(Roles="ClinicManager")]
public class RoomController(): Controller
{
    public async Task <IActionResult> Index()
    {
        return View();
    }
    
    public async Task <IActionResult> Availability()
    {
        return View();
    }
    
    public async Task <IActionResult> List()
    {
        return View();
    }
    
    public async Task <IActionResult> ViewRoom()
    {
        return View();
    }
}
