#nullable disable

using System.ComponentModel.DataAnnotations;
using Hbk.Common.Globals;
using Hbk.Database;
using Hbk.Models.DTO;
using Hbk.Platform.Helpers;
using Hbk.Platform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace Hbk.Platform.Areas.Account.Pages;

[EnableRateLimiting("clinic-registration")]
public class RegisterClinicModel(
    IClinicRegistrationService registrationService,
    SignInManager<User> signInManager,
    ILogger<RegisterClinicModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Title")]
        public Enums.Title ManagerTitle { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Forename")]
        public string ManagerForename { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Surname")]
        public string ManagerSurname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string ManagerEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Clinic name")]
        public string ClinicName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Clinic contact email")]
        public string ClinicEmail { get; set; }

        [Required]
        [Phone]
        public string Telephone { get; set; }

        [StringLength(500)]
        [Display(Name = "Clinic address")]
        public string StreetAddress { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(Input.ManagerTitle))
        {
            ModelState.AddModelError("Input.ManagerTitle", "Select a valid title.");
        }

        if (ModelState.IsValid)
        {
            var result = await registrationService.RegisterAsync(
                new ClinicRegistrationRequest(
                    Input.ManagerTitle,
                    Input.ManagerForename,
                    Input.ManagerSurname,
                    Input.ManagerEmail,
                    Input.Password,
                    Input.ClinicName,
                    Input.ClinicEmail,
                    Input.Telephone,
                    Input.StreetAddress),
                cancellationToken);

            if (result.IdentityResult.Succeeded)
            {
                var loginUser = new UserDto
                {
                    TenancyId = result.TenancyId!.Value,
                    ClinicId = result.ClinicId
                };
                await signInManager.SignInWithClaimsAsync(
                    result.User!,
                    isPersistent: false,
                    AuthenticationHelper.GetClaimsForUser(loginUser));
                logger.LogInformation(
                    "Clinic manager registered clinic {ClinicId}",
                    result.ClinicId);
                return LocalRedirect(Url.Content("~/Clinic/Reception"));
            }

            foreach (var error in result.IdentityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}
