/******************************
* HBK Home Controller
* Index route.
*
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using System.Diagnostics;
using System.Text.Encodings.Web;
using HBKPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

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


    [HttpPost]
    public string Search(string query)
    {
        return HtmlEncoder.Default.Encode(query); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

