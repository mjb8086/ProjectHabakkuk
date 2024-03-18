using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Client.Controllers
{
    /// <summary>
    /// HBKPlatform Client Reception Controller.
    /// Default landing page and other related views.
    /// 
    /// Author: Mark Brown
    /// Authored: 19/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    [Area("Client"), Authorize(Roles="Client")]
    public class ReceptionController(IPracticeService practiceService): Controller
    {
        public async Task <IActionResult> Index()
        {
            var data = await practiceService.GetClientReceptionData();
            return View(data);
        }
    }
}