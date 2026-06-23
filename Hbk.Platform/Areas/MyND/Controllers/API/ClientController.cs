using Hbk.Platform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Areas.MyND.Controllers.API;

/// <summary>
/// Hbk.Platform Client API controller.
/// 
/// Author: Mark Brown
/// Authored: 22/05/2024
/// 
/// © 2024 NowDoctor Ltd.
/// </summary>
[Area("MyND"), Authorize(Roles="Practitioner"), Route("/api/mynd/client/[action]")]
public class ClientController(IClientDetailsService _cdSrv): Controller
{
    public async Task<IActionResult> GetLite()
    {
        return Ok(await _cdSrv.GetClientsLite());
    }
}