/******************************
* HBK Home Controller
* Index route.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.Diagnostics;
using HBKPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers
{
    /// <summary>
    /// HBKPlatform Home Controller.
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

