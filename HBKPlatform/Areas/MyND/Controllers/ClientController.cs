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
public class ClientController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult ClientDetails()
    {
        return View();
    }
    
}