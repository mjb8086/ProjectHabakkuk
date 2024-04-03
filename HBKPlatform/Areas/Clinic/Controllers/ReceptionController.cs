using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Clinic.Controllers
{
    /// <summary>
    /// HBKPlatform Clinic Reception Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 20/03/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("Clinic"), Authorize(Roles="ClinicManager")]
    public class ReceptionController(): Controller
    {
        public async Task <IActionResult> Index()
        {
            return View();
        }
    }
}