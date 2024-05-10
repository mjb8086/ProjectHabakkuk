using HBKPlatform.Controllers.Common;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HBKPlatform.Areas.MyND.Controllers.API
{
    /// <summary>
    /// HBKPlatform Reception API Controller.
    /// 
    /// Author: Mark Brown
    /// Authored: 08/05/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    
    [Authorize(Roles="Practitioner"), Route("/api/mynd/[action]")]
    public class ReceptionController (IReceptionService _reception): Controller
    {
        /// <summary>
        /// Get reception summary data.
        /// </summary>
        public async Task<IActionResult> GetReceptionSummary()
        {
            return Ok(await _reception.GetReceptionSummaryData());
        }
    }
}