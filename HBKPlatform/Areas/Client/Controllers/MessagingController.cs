using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.Client.Controllers
{
    /// <summary>
    /// HBKPlatform Client Messaging Controller.
    /// All views and API routes for Clients' messaging.
    /// 
    /// Author: Mark Brown
    /// Authored: 19/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    [Area("Client"), Authorize(Roles="Client")]
    public class MessagingController
        (IClientMessagingService _clientMessagingService, ICacheService _cache, IUserService _userService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var leadId = _cache.GetLeadPractitionerId(_userService.GetClaimFromCookie("PracticeId"));
            return RedirectToRoute(new { controller = "Messaging", action = "Conversation", PracId = leadId });
        }

        [HttpGet]
        public async Task<IActionResult> Conversation(int pracId)
        {
            var convoModel = await _clientMessagingService.GetConversationClient(pracId, 10);
            return View(convoModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int pracId, [FromForm] string messageBody)
        {
            await _clientMessagingService.SendMessage(messageBody, pracId);
            return RedirectToRoute(new { controller = "Messaging", action = "Conversation", PracId = pracId });
        }
    }
}