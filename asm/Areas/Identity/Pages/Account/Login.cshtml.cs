// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using asm.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace asm.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        ///ad them
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger,
        //         UserManager<IdentityUser> userManager,
        //                     RoleManager<IdentityRole> roleManager)
        // {
        //     _signInManager = signInManager;
        //     _logger = logger;
        //     _userManager = userManager;
        //     _roleManager = roleManager;

        // }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                // if (result.Succeeded)
                // {
                //     _logger.LogInformation("User logged in.");
                //     var user = await _userManager.FindByEmailAsync(Input.Email);
                //     var roles = await _userManager.GetRolesAsync(user);

                //     if (roles.Contains("Admin"))
                //     {
                //         // Nếu người dùng có vai trò Admin, chuyển hướng đến trang quản trị
                //         return RedirectToAction("Index", "Dashboard");
                //     }
                //     else if (roles.Contains("Customer"))
                //     {
                //         // Nếu người dùng có vai trò Customer, chuyển hướng đến trang chính
                //         return RedirectToAction("Index", "Home");
                //     }
                //     // return LocalRedirect(returnUrl);
                // }

                if (result.Succeeded)
                {
                    // Lấy thông tin người dùng sau khi đăng nhập thành công
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    // Kiểm tra trạng thái người dùng
                    if (user.Status == 1)
                    {
                        _logger.LogInformation("User logged in.");

                        var roles = await _userManager.GetRolesAsync(user);
                        var allowedRoles = new List<string> { "Admin", "Manager", "Staff" };

                        // if (roles.Contains("Admin"))
                        if (roles.Any(r => allowedRoles.Contains(r)))
                        {
                            // Nếu người dùng có vai trò Admin, chuyển hướng đến trang quản trị
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else if (roles.Contains("Customer"))
                        {
                            // Nếu người dùng có vai trò Customer, chuyển hướng đến trang chính
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        // Trả về thông báo nếu tài khoản bị khoá
                        // ModelState.AddModelError(string.Empty, "Tài khoản đã bị khoá.");
                        TempData["Blocks"] = "Your account is currently locked, please contact management to resolve";
                        ViewData["Block"] = TempData["Blocks"];
                        await _signInManager.SignOutAsync(); // Đăng xuất người dùng để tránh trường hợp họ đăng nhập thành công
                        return Page();
                    }
                }


                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}