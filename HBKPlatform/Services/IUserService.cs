using HBKPlatform.Models.DTO;
using HBKPlatform.Models.View.MCP;

namespace HBKPlatform.Services;

public interface IUserService
{
    public Task<UserDto> GetClientOrPracIdForUserId(string userId);
    public int GetClaimFromCookie(string claim);
    public Task DoUacAction(UacRequest model);
}