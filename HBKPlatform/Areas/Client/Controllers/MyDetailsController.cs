using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Client.Controllers;

/// <summary>
/// HBKPlatform Client details controller.
/// 
/// Author: Mark Brown
/// Authored: 06/02/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

[Area("Client"), Authorize(Roles="Client")]
public class MyDetailsController(IClientDetailsService _cdSrv) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _cdSrv.GetClientAsClient());
    }
    
    // TODO: Make PUT
    [HttpPost]
    public async Task<IActionResult> DoEditDetails([FromForm] ClientDto client)
    {
        if (!ModelState.IsValid) throw new Exception("Model bad");
        await _cdSrv.UpdateClientDetailsAsClient(client);
        TempData["Message"] = "Successfully updated your details";
        return RedirectToRoute(new { controller = "MyDetails", action = "Index" });
    }
    
}