using Hbk.Models.API.API.Common;
using Hbk.Models.DTO;
using Hbk.Models.View.MCP;

namespace Hbk.Platform.Services
{
    public interface IUserService
    {
        public Task<UserDto> GetLoginUserDto(string userId);
        public int GetClaimFromCookie(string claim);
        public Task DoUacAction(UacRequest model);
        public Task<UserData> GetCurrentUserData();
    }
}
