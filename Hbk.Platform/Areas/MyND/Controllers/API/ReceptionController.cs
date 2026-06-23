using Hbk.Platform.Services;
using Hbk.Platform.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Areas.MyND.Controllers.API
{
    /// <summary>
    /// Hbk.Platform Reception API Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 08/05/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    
    [Area("MyND"), Authorize(Roles="Practitioner"), Route("/api/mynd/reception/[action]")]
    public class ReceptionController (IReceptionService _reception): Controller
    {
        /// <summary>
        /// Get reception summary data.
        /// </summary>
        public async Task<IActionResult> Summary()
        {
            return Ok(await _reception.GetReceptionSummaryData());
        }
    }
}