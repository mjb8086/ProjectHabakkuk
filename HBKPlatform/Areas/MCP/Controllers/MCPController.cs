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
[Area("MCP"), Route(("mcp")), Authorize]
public class MCPController : Controller
{
    // GET: Index
    public async Task<IActionResult> Index()
    {
        return View();
    }

}
