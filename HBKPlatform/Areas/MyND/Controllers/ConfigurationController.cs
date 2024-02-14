using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

[Area("MyND"), Authorize(Roles="Practitioner")]
public class ConfigurationController : Controller
{
   public IActionResult Index()
   {
      return View();
   }
}