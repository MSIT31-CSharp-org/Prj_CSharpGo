﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Areas.Identity.Data;

namespace Prj_CSharpGo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<identityForUser> _signInManager;
        private readonly UserManager<identityForUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<identityForUser> userManager,
            SignInManager<identityForUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "請輸入Email")]
            [EmailAddress(ErrorMessage = "請輸入正確Email格式(例如. xxx@example.com)")]
            //[Display(Name = "請輸入帳號(Email格式)")]
            public string Email { get; set; }

            [Required(ErrorMessage = "請輸入密碼")]
            [StringLength(100, ErrorMessage = "密碼請輸入至少{2}位英數字", MinimumLength = 6)]
            [DataType(DataType.Password)]
            //[Display(Name = "請輸入密碼")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            //[Display(Name = "確認密碼")]
            [Compare("Password", ErrorMessage = "前後密碼不一致")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new identityForUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("使用者您好，請註冊一組專屬您的會員資格");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "會員註冊驗證信",
                        $"<h1><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處鏈結完成電子郵件驗證</a></h1>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }

    //為 ASP.NET Core Identity 編寫自定義驗證器
    //public interface IPasswordValidator<TUser> where TUser : class
    //{
    //    Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password);
    //}
    //public class UsernameAsPasswordValidator<TUser> : IPasswordValidator<TUser>
    //where TUser : IdentityUser
    //{
    //    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
    //    {
    //        if (string.Equals(user.UserName, password, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return Task.FromResult(IdentityResult.Failed(new IdentityError
    //            {
    //                Code = "UsernameAsPassword",
    //                Description = "You cannot use your username as your password"
    //            }));
    //        }
    //        return Task.FromResult(IdentityResult.Success);
    //    }
    //}

    //public class UsernameAsPasswordValidator : IPasswordValidator<ApplicationUser>
    //{
    //    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
    //    {
    //        // as before
    //    }
    //}
}
