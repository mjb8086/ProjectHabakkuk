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
    public class UserService(ApplicationDbContext _db, IHttpContextAccessor _httpContext, IUserRepository _userRepo, ILogger<UserService> _logger) : IUserService
    {
        // TODO: Refactor these into repositories.
        
        /// <summary>
        /// use this to set the user's associated prac Id or client Id, also update last sign in time
        /// used when signing in and set in user claims (cookie)
        /// </summary>
        public async Task<UserDto> GetLoginUserDto(string userId)
        {
            var user = new UserDto();
            var client = await _db.Clients.Include("User").IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId);    // TODO: Put these in repositories
            Practitioner? prac;
            User? dbUser;

            if (client != null)
            {
                user.ClientId = client.Id;
                user.ClinicId = client.ClinicId;
                user.TenancyId = client.TenancyId;
                client.User.LastLogin = DateTime.UtcNow;
                client.User.LoginCount++;
            }
            else if ((prac = await _db.Practitioners.Include("User").IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId)) != null)
            {
                user.PractitionerId = prac.Id;
                user.ClinicId = prac.ClinicId;
                user.TenancyId = prac.TenancyId;
                prac.User.LastLogin = DateTime.UtcNow;
                prac.User.LoginCount++;
            }
            else if((dbUser = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId)) != null) // user has neither client or prac entity, get tenancyId from user entity
            {
                user.TenancyId = dbUser.TenancyId;
                dbUser.LastLogin = DateTime.UtcNow;
                dbUser.LoginCount++;
            }

            await _db.SaveChangesAsync();
            return user;
        }
    
        private async Task<string> GetPracUserId(int pracId)
        {
            var client = await _db.Practitioners.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == pracId);
            if (client == null || string.IsNullOrWhiteSpace(client.UserId))
            {
                throw new MissingMemberException("UserID is null");
            }
            return client.UserId;
        }
    
        public async Task DoUacAction(UacRequest model)
        {
            var userId = await GetPracUserId(model.PractitionerId);
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
