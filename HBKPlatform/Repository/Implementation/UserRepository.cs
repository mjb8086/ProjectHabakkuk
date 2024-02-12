using System.Data;
using HBKPlatform.Database;
using HBKPlatform.Globals;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Repository.Implementation;

public class UserRepository(ApplicationDbContext _db, IPasswordHasher<User> passwordHasher) : IUserRepository
{
    public async Task ResetPasswordForUser(string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId) 
                   ?? throw new MissingPrimaryKeyException($"Could not find user ID {userId}");
        
        var pwdGen = new PasswordGenerator.Password(DefaultSettings.DEFAULT_PASSWORD_LENGTH);
        var pwd = pwdGen.Next();
        // TODO: REMOVE THIS!!!
        Console.WriteLine($"DEBUG: PASSWORD IS ====>\n{pwd}\n");
        user.PasswordHash = passwordHasher.HashPassword(user, pwd);
        
        await _db.SaveChangesAsync();
    }

    public async Task ToggleLockout(string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId) 
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
    
}