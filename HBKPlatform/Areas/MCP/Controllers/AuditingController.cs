using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MCP.Controllers
{
    /// <summary>
    /// HBKPlatform MCP auditing Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 14/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("MCP"), Authorize(Roles="SuperAdmin")]
    public class AuditingController(ICacheService _cacheService, ICentralScrutinizerService _centralScrutinizerService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    
        public async Task<IActionResult> WhoIsOnline()
        {
            return View(_centralScrutinizerService.GetActive());
        }

        public IActionResult ClearCache()
        {
            _cacheService.ClearAll();
            TempData["Message"] = "Successfully cleared cache for all tenants.";
            return RedirectToRoute(new { controller = "Auditing", action = "Index" });
        }

    }
}
