using Hbk.Platform.Models.API.Common;
using Hbk.Platform.Models.DTO;
using Hbk.Platform.Models.View.MCP;

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
