using HBKPlatform.Models.DTO;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers
{
   [Area("MyND"), Authorize(Roles="Practitioner")]
   public class ConfigurationController(IConfigurationService _config) : Controller
   {
      public IActionResult Index()
      {
         return View();
      }

      [HttpPut]
      public async Task UpdateSetting(string key, string value)
      {
         await _config.UpdateSetting(key, value);
      }
      
      [HttpPost]
      public async Task CreateSetting(SettingDto setting)
      {
         await _config.CreateSetting(setting);
      }
   }
}