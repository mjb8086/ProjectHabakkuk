using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
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
public class ClientController(IClientDetailsService _cdSrv) : Controller
{
    public IActionResult Index()
    {
        return View();
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
    public async Task<IActionResult> DoAddEditClient(int? clientId)
    {
        return Ok("wip");
    }
    
}