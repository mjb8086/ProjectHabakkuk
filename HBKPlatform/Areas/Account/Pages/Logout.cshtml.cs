// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using HBKPlatform.Database;
using HBKPlatform.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HBKPlatform.Areas.Account.Pages
{
    public class LogoutModel (SignInManager<User> _signInManager, ILogger<LogoutModel> _logger, 
                                         ICentralScrutinizerService _centralScrutinizer, IHttpContextAccessor _httpCtx): PageModel
    {
        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (_httpCtx.HttpContext != null) _centralScrutinizer.RemoveUser(_httpCtx.HttpContext);
            // This needs to be a redirect so that the browser performs a new request and the identity for the user gets updated.
            return Redirect("/");
        }
    }
}
