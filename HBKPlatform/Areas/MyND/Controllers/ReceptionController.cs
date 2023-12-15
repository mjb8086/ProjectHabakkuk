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
public class ReceptionController: Controller
{
    public IActionResult Index()
    {
        return View();
    }
}