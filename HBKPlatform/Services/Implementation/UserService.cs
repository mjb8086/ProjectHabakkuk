using HBKPlatform.Database;
using HBKPlatform.Models.DTO;
using HBKPlatform.Repository;
using Microsoft.EntityFrameworkCore;

namespace HBKPlatform.Services.Implementation;

public class UserService(ApplicationDbContext _db): IUserService
{
    /// <summary>
    /// use this to set the user's associated prac Id or client Id
    /// used when signing in and set in user claims (cookie)
    /// </summary>
    public async Task<UserDto> GetClientOrPracIdForUserId(string userId)
    {
        var user = new UserDto();
        var client = await _db.Clients.FirstOrDefaultAsync(x => x.UserId == userId);
        Practitioner prac;
        
        if (client != null)
        {
            user.ClientId = client.Id;
            user.ClinicId = client.ClinicId;
        }
        else if((prac = await _db.Practitioners.FirstOrDefaultAsync(x => x.UserId == userId)) != null)
        {
            user.PractitionerId = prac.Id;
            user.ClinicId = prac.ClinicId;
        }
        return user;
    }
}