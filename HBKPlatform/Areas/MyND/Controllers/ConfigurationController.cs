using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

[Area("MyND")]
public class ConfigurationController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}