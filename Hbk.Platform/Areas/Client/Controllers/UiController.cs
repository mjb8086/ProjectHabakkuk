using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Areas.Client.Controllers
{
    /// <summary>
    /// Hbk.Platform Client UI Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 26/04/24
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("Client"), Authorize(Roles="Client")]
    public class UiController(): Controller
    {
        public async Task <IActionResult> Index()
        {
            return View();
        }
    }
}