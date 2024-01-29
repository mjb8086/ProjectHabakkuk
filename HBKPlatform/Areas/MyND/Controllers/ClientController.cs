using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform Client controller.
/// 
/// Author: Mark Brown
/// Authored: 10/01/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>

[Area(("MyND"))]
[Authorize]
public class ClientController(IClientDetailsService _cdSrv) : Controller
{
    public IActionResult Index()
    {
        return View(_cdSrv.GetClientDetailsIndex());
    }
    
    public async Task<IActionResult> AllClients()
    {
        return View(await _cdSrv.GetAllClientsView());
    }
    
    public async Task<IActionResult> AddEditClient(int? clientId)
    {
        return View(clientId.HasValue ? await _cdSrv.GetClient(clientId.Value) : new ClientDto());
    }

    [HttpPost]
    public async Task<IActionResult> DoAddClient([FromForm] ClientDto client)
    {
        await _cdSrv.CreateClient(client);
        TempData["Message"] = "New client registered. An activation email will be sent to his/her inbox.";
        
        return RedirectToRoute(new { controller = "Client", action = "AllClients" });
    }
    
    // TODO: Make PUT
    [HttpPost]
    public async Task<IActionResult> DoEditClient(int clientId, [FromForm] ClientDto client)
    {
        client.Id = clientId;
        await _cdSrv.UpdateClientDetails(client);
        TempData["Message"] = "Successfully updated client details";
        return RedirectToRoute(new { controller = "Client", action = "AllClients" });
    }
    
}