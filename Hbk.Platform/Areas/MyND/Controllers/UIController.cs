using Hbk.Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Areas.MyND.Controllers
{
    /// <summary>
    /// Hbk.Platform MyND New UI Controller.
    /// Boot up the SPA and also ensure it receives the routes that match this index.
    /// The things we do to tick the boxes imposed by modern web app dev, eh?
    /// 
    /// Author: Mark Brown
    /// Authored: 26/04/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("MyND"), Authorize(Roles="Practitioner")]
    public class UIController(): Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}