/******************************
* HBK AdminPanel Controller
* Author: Mark Brown
* Authored: 10/09/2022
******************************/

using HBKPlatform.Database;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MasterControlPanel.Controllers
{
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
}
