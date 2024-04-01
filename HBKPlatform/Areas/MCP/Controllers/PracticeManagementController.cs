using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MCP.Controllers
{
    /// <summary>
    /// HBKPlatform MCP Practice management controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 07/02/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    [Area("MCP"), Authorize(Roles="SuperAdmin")]
    public class PracticeManagementController(IMcpService _mcpService, IUserService _userService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    
        public async Task<IActionResult> ViewPractice(int practiceId)
        {
            return View(await _mcpService.GetPracticeModel(practiceId));
        }

        [HttpPost]
        public async Task<IActionResult> DoUpdatePractice(int practiceId, [FromForm] PracticeDto model)
        {
            model.Id = practiceId;
            if (!ModelState.IsValid) throw new MissingFieldException("all your fields are belong to us");
            await _mcpService.UpdatePractice(model);
            return RedirectToRoute(new { controller = "PracticeManagement", action = "ViewPractice", practiceId=practiceId });
        }
    
        public async Task<IActionResult> ListPractices()
        {
            return View(await _mcpService.GetListPracticesView());
        }

        public async Task<IActionResult> PasswordReset()
        {
            return View(await _mcpService.GetUacViewPractices());
        }
    
        public async Task<IActionResult> RegisterPractice()
        {
            return View();
        }
    
        [HttpPost]
        public async Task<IActionResult> DoRegisterPractice([FromForm] PracticeRegistrationDto model)
        {
            if (!ModelState.IsValid) throw new MissingFieldException("Someone set us up the model");
            await _mcpService.RegisterPractice(model);
            TempData["Message"] = "Successfully registered new practice and created lead practitioner user.";
            return RedirectToRoute(new { controller = "PracticeManagement", action = "ListPractices" });
        }

        [HttpPost]
        public async Task<IActionResult> DoUacAction([FromForm] UacRequest model)
        {
            if (!ModelState.IsValid) throw new MissingFieldException("The model is bad gentlemen make your time");
            await _userService.DoUacAction(model);
            TempData["Message"] = $"Successfully completed action {model.Action}.";
            return RedirectToRoute(new { controller = "PracticeManagement", action = "PasswordReset" });
        }

        // API METHODS
        public async Task<IActionResult> GetPracticePracs(int practiceId)
        {
            return Ok(await _mcpService.GetPracPracs(practiceId));
        }


    }
}
