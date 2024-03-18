using System.Security.Claims;
using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Repository;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Services.Implementation
{
    /// <summary>
    /// HBKPlatform User service.
    /// Middleware for user controller and database functionality.
    /// 
    /// Author: Mark Brown
    /// Authored: 15/12/2023
    /// 
    /// © 2023 NowDoctor Ltd.
    /// </summary>
    public class UserService(IHttpContextAccessor _httpContext, IUserRepository _userRepo, IMcpRepository _mcpRepo, 
        ILogger<UserService> _logger) : IUserService
    {
        
        /// <summary>
        /// use this to set the user's associated prac Id or client Id, also update last sign in time
        /// used when signing in and set in user claims (cookie)
        /// </summary>
        public async Task<UserDto> GetLoginUserDto(string userId)
        {
            return await _userRepo.GetAndUpdateLoginUser(userId);
        }
    
    
        public async Task DoUacAction(UacRequest model)
        {
            var userId = await _mcpRepo.GetPracUserId(model.PractitionerId);
            switch (model.Action)
            {
                case UacAction.PasswordReset:
                    await _userRepo.ResetPasswordForUser(userId);
                    _logger.LogInformation($"UAC: Password reset performed for {userId}");
                    break;
                case UacAction.ToggleLockout:
                    await _userRepo.ToggleLockout(userId);
                    _logger.LogInformation($"UAC: Lockout toggled for {userId}");
                    break;
            }
        }

        /// <summary>
        /// Get the specified claim from the cookie, converts to integer
        /// </summary>
        /// <returns>claim value as integer on success, or -1 on failure</returns>
        public int GetClaimFromCookie(string claim)
        {
            var c = _httpContext.HttpContext?.User.FindFirst(claim);
            if (c != null && int.TryParse(c.Value, out int val))
            {
                return val;
            }
            _logger.LogWarning($"User {_httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)} attempted to retrieve missing claim: {claim}");
            return -1;
        }
    
    }
}
