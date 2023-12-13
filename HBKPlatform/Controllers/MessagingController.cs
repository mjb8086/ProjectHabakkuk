using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

public class MessagingController(IClientMessagingService _clientMessagingService): Controller
{
    public async Task<IActionResult> GetConversationView()
    {
        var convoModel = _clientMessagingService.GetConversation("", "");
        return Ok();
    }
}