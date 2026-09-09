// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineBankingApplication.Data;
using OnlineBankingApplication.Models;

using ApplicationUser = OnlineBankingApplication.Models.ApplicationUser;

namespace OnlineBankingApplication.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly ApplicationDbContext _context;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(
                string.Empty,
                ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        await HttpContext.SignOutAsync(
            IdentityConstants.ExternalScheme);

        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins =
            (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
            .ToList();

        if (ModelState.IsValid)
        {
            var user = await _userManager
                .FindByEmailAsync(Input.Email);

            if (user != null)
            {
                var isAdmin = await _userManager
                    .IsInRoleAsync(user, "Admin");

                if (!user.IsApproved && !isAdmin)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Your account has not been approved by the administrator.");

                    return Page();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                var loggedInUser = await _userManager
                    .FindByEmailAsync(Input.Email);

                if (loggedInUser != null)
                {
                    var auditLog = new AuditLog
                    {
                        UserId = loggedInUser.Id,
                        Action = "Login",
                        Description = "User logged in successfully.",
                        CreatedDate = DateTime.Now
                    };

                    _context.AuditLogs.Add(auditLog);

                    await _context.SaveChangesAsync();

                    // Admin → Admin Dashboard
                    if (await _userManager.IsInRoleAsync(
                        loggedInUser,
                        "Admin"))
                    {
                        return RedirectToAction(
                            "Index",
                            "Dashboard");
                    }

                    // Customer → Customer Dashboard
                    return RedirectToAction(
                        "Index",
                        "CustomerDashboard");
                }

                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage(
                    "./LoginWith2fa",
                    new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = Input.RememberMe
                    });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(
                    "User account locked out.");

                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid login attempt.");

                return Page();
            }
        }

        return Page();
    }
}