/******************************
* HBK Home Controller
* Index route.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.Diagnostics;
using Hbk.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Controllers
{
    /// <summary>
    /// Hbk.Platform Home Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 10/09/2022
    /// 
    /// © 2022 NowDoctor Ltd.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Index", "MCP", new { area = "MCP" });
                }

                if (User.IsInRole("ClinicManager"))
                {
                    return RedirectToAction("Index", "Reception", new { area = "Clinic" });
                }

                if (User.IsInRole("Practitioner"))
                {
                    return RedirectToAction("Index", "Reception", new { area = "MyND" });
                }

                if (User.IsInRole("Client"))
                {
                    return RedirectToAction("Index", "Reception", new { area = "Client" });
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorCode = Response?.StatusCode.ToString()});
        }
    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ErrorDev()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorCode = Response?.StatusCode.ToString()});
        }
    }
}
