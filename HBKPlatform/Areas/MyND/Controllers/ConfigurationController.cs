using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers
{
   [Area("MyND"), Authorize(Roles="Practitioner")]
   public class ConfigurationController : Controller
   {
      public IActionResult Index()
      {
         return View();
      }
   }
}