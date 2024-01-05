using HBKPlatform.Models.DTO;

namespace HBKPlatform.Services;

public interface IUserService
{
    public Task<UserDto> GetClientOrPracIdForUserId(string userId);
    public int GetClaimFromCookie(string claim);
}