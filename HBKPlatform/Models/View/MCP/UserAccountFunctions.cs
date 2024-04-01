using HBKPlatform.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBKPlatform.Models.View.MCP
{
    public class UserAccountFunctions
    {
        public List<SelectListItem> Practices { get; set; }
        public List<SelectListItem> Clinics { get; set; }
    }

    public class UacRequest
    {
        public int? PractitionerId { get; set; }
        public int? ClinicId { get; set; }
        public int? ClientId { get; set; }
        public UacAction Action { get; set; }
    }


    public enum UacAction
    {
        PasswordReset = 1,
        ToggleLockout = 2
    }
    

    public struct UacUserSelect
    {
        public Dictionary<int, UserDetailsUac> Users { get; set; }
    }
}