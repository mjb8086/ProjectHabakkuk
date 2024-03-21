using System.Data;
using HBKPlatform.Database;
using HBKPlatform.Exceptions;
using HBKPlatform.Globals;
using HBKPlatform.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation
{
    public class UserRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher, IHttpContextAccessor _ctx, ILogger<UserRepository> _logger) : IUserRepository
    {
        /// <summary>
        /// Reset password for the tenancy User Id. Or, any user if actioned by a Super Admin.
        /// </summary>
        public async Task ResetPasswordForUser(string userId)
        {
            IQueryable<User> userQuery = _ctx.HttpContext != null && _ctx.HttpContext.User.IsInRole("SuperAdmin") ?
                _db.Users.IgnoreQueryFilters() : _db.Users;
            
            var user = await userQuery.FirstOrDefaultAsync(x => x.Id == userId) 
                       ?? throw new IdxNotFoundException($"Could not find user ID {userId}");
        
            var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
            var pwd = pwdGen.Next();
            // TODO: REMOVE THIS!!!
            Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
            user.PasswordHash = passwordHasher.HashPassword(user, pwd);
        
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Permanently Lockout the tenancy User Id. Or, any user if actioned by a Super Admin.
        /// </summary>
        public async Task ToggleLockout(string userId)
        {
            IQueryable<User> userQuery = _ctx.HttpContext != null && _ctx.HttpContext.User.IsInRole("SuperAdmin") ?
                _db.Users.IgnoreQueryFilters() : _db.Users;
        
            var user = await userQuery.FirstOrDefaultAsync(x => x.Id == userId) 
                       ?? throw new IdxNotFoundException($"Could not find user ID {userId}");

            // Lockout enabled means the user has the *ability* to be locked out. It is not a lockout *toggle*. Ffs.
            if (user.LockoutEnabled && (!user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.UtcNow))
            {
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.AddYears(100));
            }
            else if(user.LockoutEnabled)
            {
                user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.Subtract(new TimeSpan(365, 0, 0, 0)));
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
        
        // TODO: Use cache if this becomes a regular op. Currently only used in messaging service
        public async Task<bool> VerifyClientPractitionerMembership(int clientId, int practitionerId)
        {
            var client = await _db.Clients.FirstAsync(x => x.Id == clientId);
            var prac = await _db.Practitioners.FirstAsync(x => x.Id == practitionerId);
            return client.PracticeId == prac.PracticeId && client.TenancyId == prac.TenancyId;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // BEGIN LOGIN METHODS - no tenancy ID filtering. Do not use except on login!
        ////////////////////////////////////////////////////////////////////////////////
        public async Task<UserDto> GetAndUpdateLoginUser(string userId)
        {
            var user = new UserDto();
            var client = await _db.Clients.Include("User").IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId);    // TODO: Put these in repositories
            Practitioner? prac;
            User? dbUser;
            Clinic? clinic;

            if (client != null)
            {
                user.ClientId = client.Id;
                user.PracticeId = client.PracticeId;
                user.TenancyId = client.TenancyId;
                client.User.LastLogin = DateTime.UtcNow;
                client.User.LoginCount++;
            }
            else if ((prac = await _db.Practitioners.Include("User").IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId)) != null)
            {
                user.PractitionerId = prac.Id;
                user.PracticeId = prac.PracticeId;
                user.TenancyId = prac.TenancyId;
                prac.User.LastLogin = DateTime.UtcNow;
                prac.User.LoginCount++;
            }
            else if ((clinic = await _db.Clinics.Include("ManagerUser").IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ManagerUserId == userId)) != null)
            {
                user.TenancyId = clinic.TenancyId;
                user.ClinicId = clinic.Id;
                clinic.ManagerUser.LastLogin = DateTime.UtcNow;
                clinic.ManagerUser.LoginCount++;
            }
            else if((dbUser = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == userId)) != null) // user has neither client or prac entity, get tenancyId from user entity
            {
                user.TenancyId = dbUser.TenancyId;
                dbUser.LastLogin = DateTime.UtcNow;
                dbUser.LoginCount++;
                _logger.LogWarning($"Plain user with no associated Client, Practitioner or Clinic login: {userId}");
            }

            await _db.SaveChangesAsync();
            return user;
        }
        
        ////////////////////////////////////////////////////////////////////////////////
        // END LOGIN METHODS 
        ////////////////////////////////////////////////////////////////////////////////
    
    }
}
