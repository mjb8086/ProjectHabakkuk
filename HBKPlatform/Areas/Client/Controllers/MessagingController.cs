using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Client.Controllers;

/// <summary>
/// HBKPlatform Client Messaging Controller.
/// All views and API routes for Clients' messaging.
/// 
/// Author: Mark Brown
/// Authored: 19/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
[Area("Client")]
public class MessagingController
    (IClientMessagingService _clientMessagingService, ICacheService _cache, IHttpContextAccessor _httpContextAccessor) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var clinicIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("ClinicId");
        if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out int clinicId))
        {
            // TODO crude - fix later when we have a direct client-prac relationship
            var pracs = await _cache.GetClinicPracDetailsLite(clinicId);
            return Redirect($"/client/messaging/conversation?pracId={pracs.First().Id}"); // also replace with controller-based routing when understood
        }
        return NotFound("No conversation exists");
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
        return Redirect($"conversation?pracId={pracId}");
    }
}