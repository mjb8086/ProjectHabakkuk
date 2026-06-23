using Hbk.Platform.Services;
using Hbk.Platform.Models.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hbk.Platform.Controllers.Common
{
    /// <summary>
    /// Hbk.Platform Common User API Controller.
    /// User data routes shared between all sections of the app.
    /// 
    /// Author: Mark Brown
    /// Authored: 02/05/2024
    /// 
    /// © 2024 NowDoctor Ltd.
    /// </summary>
    
    [Authorize(Roles="Practitioner,Client,Clinic,SuperAdmin"), Route("/api/common/[action]")]
    public class UserController (ILogger<UserController> _logger, IUserService _userService): Controller
    {
        /// <summary>
        /// Get data for the current user.
        /// </summary>
        public async Task<IActionResult> GetUserData()
        {
            return Ok(await _userService.GetCurrentUserData());
        }

        /// <summary>
        /// API Test Method
        /// </summary>
        public IActionResult Nothing()
        {
            return Ok("goodbye world");
        }
    }
}

