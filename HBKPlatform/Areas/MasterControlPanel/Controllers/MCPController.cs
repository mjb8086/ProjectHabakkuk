using HBKPlatform.Database;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MasterControlPanel.Controllers;

/// <summary>
/// HBKPlatform MCP Controller.
/// 
/// Author: Mark Brown
/// Authored: 11/12/2023
/// 
/// © 2023 NowDoctor Ltd.
/// </summary>
[Area("MasterControlPanel")]
[Route(("mcp"))]
public class MCPController : Controller
{
    private readonly ApplicationDbContext _context;

    public MCPController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Index
    public async Task<IActionResult> Index()
    {
        return View();
    }

}
