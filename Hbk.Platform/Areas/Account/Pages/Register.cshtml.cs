// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Hbk.Common.Globals;
using Hbk.Database;
using Hbk.Platform.Helpers;
using Hbk.Platform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace Hbk.Platform.Areas.Account.Pages
{
    [EnableRateLimiting("practitioner-registration")]
    public class RegisterModel(
        IPractitionerRegistrationService registrationService,
        SignInManager<User> signInManager,
        ILogger<RegisterModel> logger) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public Enums.Title Title { get; set; }

            [Required]
            [StringLength(100)]
            public string Forename { get; set; }

            [Required]
            [StringLength(100)]
            public string Surname { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of birth")]
            public DateOnly? DateOfBirth { get; set; }

            [Required]
            [StringLength(200)]
            [Display(Name = "Practice name")]
            public string PracticeName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Practice contact email")]
            public string PracticeEmail { get; set; }

            [Required]
            [Phone]
            public string Telephone { get; set; }

            [StringLength(500)]
            [Display(Name = "Practice address")]
            public string StreetAddress { get; set; }

            [StringLength(200)]
            [Display(Name = "Practice tagline")]
            public string PracticeTagline { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(Input.Title))
            {
                ModelState.AddModelError("Input.Title", "Select a valid title.");
            }

            if (Input.DateOfBirth is not null && Input.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                ModelState.AddModelError("Input.DateOfBirth", "Date of birth cannot be in the future.");
            }

            if (ModelState.IsValid)
            {
                var result = await registrationService.RegisterAsync(
                    new PractitionerRegistrationRequest(
                        Input.Title,
                        Input.Forename,
                        Input.Surname,
                        Input.DateOfBirth!.Value,
                        Input.Email,
                        Input.Password,
                        Input.PracticeName,
                        Input.PracticeEmail,
                        Input.Telephone,
                        Input.StreetAddress,
                        Input.PracticeTagline),
                    cancellationToken);

                if (result.IdentityResult.Succeeded)
                {
                    var loginUser = new Hbk.Models.DTO.UserDto
                    {
                        TenancyId = result.TenancyId!.Value,
                        PracticeId = result.PracticeId,
                        PractitionerId = result.PractitionerId
                    };
                    await signInManager.SignInWithClaimsAsync(
                        result.User!,
                        isPersistent: false,
                        AuthenticationHelper.GetClaimsForUser(loginUser));
                    logger.LogInformation(
                        "Practitioner {PractitionerId} registered practice {PracticeId}",
                        result.PractitionerId,
                        result.PracticeId);
                    return LocalRedirect(Url.Content("~/MyND/Reception"));
                }

                foreach (var error in result.IdentityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
