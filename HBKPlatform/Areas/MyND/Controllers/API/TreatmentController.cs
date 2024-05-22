using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers.API;

/// <summary>
/// HBKPlatform Treatment API controller.
/// 
/// Author: Mark Brown
/// Authored: 22/05/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("MyND"), Authorize(Roles="Practitioner"), Route("/api/mynd/treatment/[action]")]
public class TreatmentController(ITreatmentService _treatmentService): Controller
{
    public async Task<IActionResult> GetLite()
    {
        return Ok(await _treatmentService.GetTreatmentsLite());
    }
}