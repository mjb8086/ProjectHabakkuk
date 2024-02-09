using Microsoft.AspNetCore.Mvc.Rendering;

namespace HBKPlatform.Models.View.MCP;

public class UserAccountFunctions
{
    public List<SelectListItem> Clinics { get; set; }
    public int ClinicId { get; set; }
    public int PractitionerId { get; set; }
    public UacAction Action { get; set; }
    
}

public enum UacAction
{
    PasswordReset = 1,
    ToggleLockout = 2
}