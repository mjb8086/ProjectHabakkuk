using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

/// <summary>
/// HBKPlatform MyND Messaging Controller.
/// All views and API routes for Practitioner's messaging.
/// 
/// Author: Mark Brown
/// Authored: 13/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
[Area("MyND")]
public class MessagingController
    (IClientMessagingService _clientMessagingService, IClinicService _clinicService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Conversation(int clientId)
    {
        var convoModel = await _clientMessagingService.GetConversationPractitioner(clientId, 10);
        return View(convoModel);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var inboxView = await _clinicService.GetInboxModel();
        return View(inboxView);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(int clientId, [FromForm] string messageBody)
    {
        await _clientMessagingService.SendMessage(messageBody, clientId);
        return Redirect($"conversation?clientId={clientId}");
    }
}