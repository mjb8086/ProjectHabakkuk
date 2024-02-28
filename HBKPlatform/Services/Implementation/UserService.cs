using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;
using HBKPlatform.Repository;
using Microsoft.AspNetCore.Identity;
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
    public class UserService(ApplicationDbContext _db, IHttpContextAccessor _httpContext, IUserRepository _userRepo) : IUserService
    {
        /// <summary>
        /// use this to set the user's associated prac Id or client Id
        /// used when signing in and set in user claims (cookie)
        /// </summary>
        public async Task<UserDto> GetLoginUserDto(string userId)
        {
            var user = new UserDto();
            var client = await _db.Clients.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId);    // TODO: Put these in repositories
            Practitioner? prac;
            User? dbUser;

            if (client != null)
            {
                user.ClientId = client.Id;
                user.ClinicId = client.ClinicId;
                user.TenancyId = client.TenancyId;
            }
            else if ((prac = await _db.Practitioners.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId)) != null)
            {
                user.PractitionerId = prac.Id;
                user.ClinicId = prac.ClinicId;
                user.TenancyId = prac.TenancyId;
            }
            else if((dbUser = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId)) != null) // user has neither client or prac entity, get tenancyId from user entity
            {
                user.TenancyId = dbUser.TenancyId;
            }

            return user;
        }
    
        private async Task<string> GetPracUserId(int pracId)
        {
            var client = await _db.Practitioners.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == pracId);
            if (string.IsNullOrWhiteSpace(client.UserId))
            {
                throw new NullReferenceException("UserID is null");
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
                    break;
                case UacAction.ToggleLockout:
                    await _userRepo.ToggleLockout(userId);
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
            return -1;
        }
    
    }
}