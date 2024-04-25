using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers
{
    /// <summary>
    /// HBKPlatform MyND Reception Controller.
    /// Default landing page and other related views.
    /// 
    /// Author: Mark Brown
    /// Authored: 13/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    [Area("MyND"), Authorize(Roles="Practitioner")]
    public class ReceptionController(IPracticeService practiceService): Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await practiceService.GetPractitionerReceptionModel());
        }
        
        public async Task<IActionResult> NewUI()
        {
            return View();
        }
    }
}