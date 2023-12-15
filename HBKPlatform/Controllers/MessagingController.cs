using HBKPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Controllers;

public class MessagingController(IClientMessagingService _clientMessagingService): Controller
{
    public async Task<IActionResult> GetConversationViewClient()
    {
        var convoModel = _clientMessagingService.GetConversationClient(1,0);
        return Ok();
    }
    
    public async Task<IActionResult> GetConversationViewPractitioner()
    {
        var convoModel = _clientMessagingService.GetConversationPractitioner(1,0);
        return Ok();
    }
}