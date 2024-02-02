using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform MyND Reception Controller.
/// Default landing page and other related views.
/// 
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
[Area("MyND")]
[Authorize]
public class ReceptionController(IClinicService _clinicService): Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _clinicService.GetReceptionModel());
    }
}