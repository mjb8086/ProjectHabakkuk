using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MCP.Controllers;

/// <summary>
/// HBKPlatform MCP Controller.
/// 
/// Author: Mark Brown
/// Authored: 11/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
[Area("MCP"), Authorize(Roles="SuperAdmin")]
public class MCPController : Controller
{
    [Route("mcp")]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    [Route("mcp/status")]
    public async Task<IActionResult> Status()
    {
        return View();
    }

}
