using System.Data;
using HBKPlatform.Database;
using HBKPlatform.Globals;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    public class UserRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, IHttpContextAccessor _ctx) : IUserRepository
    {
        /// <summary>
        /// Reset password for the tenancy User Id. Or, any user if actioned by a Super Admin.
        /// </summary>
        public async Task ResetPasswordForUser(string userId)
        {
            IQueryable<User> userQuery = _ctx.HttpContext != null && _ctx.HttpContext.User.IsInRole("SuperAdmin") ?
                _db.Users.IgnoreQueryFilters() : _db.Users;
            
            var user = await userQuery.FirstOrDefaultAsync(x => x.Id == userId) 
                       ?? throw new MissingPrimaryKeyException($"Could not find user ID {userId}");
        
            var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
            var pwd = pwdGen.Next();
            // TODO: REMOVE THIS!!!
            Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
            user.PasswordHash = passwordHasher.HashPassword(user, pwd);
        
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Lockout for the tenancy User Id. Or, any user if actioned by a Super Admin.
        /// </summary>
        public async Task ToggleLockout(string userId)
        {
            IQueryable<User> userQuery = _ctx.HttpContext != null && _ctx.HttpContext.User.IsInRole("SuperAdmin") ?
                _db.Users.IgnoreQueryFilters() : _db.Users;
        
            var user = await userQuery.FirstOrDefaultAsync(x => x.Id == userId) 
                       ?? throw new MissingPrimaryKeyException($"Could not find user ID {userId}");

            if (user.LockoutEnabled)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.Subtract(new TimeSpan(365, 0, 0, 0)));
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.AddYears(100));
            }

            await _db.SaveChangesAsync();
        }
    
        /// <summary>
        /// Check all tenancies for a duplicate email. This is required because we use the email to identify the user
        /// and set his tenancy at login.
        /// If new email equals current email, allow this.
        /// </summary>
        public async Task<bool> IsEmailInUse(string newEmail, string? currentEmail = null)
        {
            if (newEmail == currentEmail) return false;
            return await _db.Users.IgnoreQueryFilters().AnyAsync(x =>
                x.Email != null && x.Email.ToLower() == newEmail || x.UserName != null && x.UserName.ToLower() == newEmail);
        }
    
    
    }
}