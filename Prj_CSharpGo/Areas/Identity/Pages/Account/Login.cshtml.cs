using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Prj_CSharpGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Prj_CSharpGo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<identityForUser> _userManager;
        private readonly SignInManager<identityForUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private WildnessCampingContext _context;


        public LoginModel(
            SignInManager<identityForUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<identityForUser> userManager,
            WildnessCampingContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "請輸入Email")]
            [EmailAddress(ErrorMessage = "請輸入正確Email格式(例如. xxx@example.com)")]
            public string Email { get; set; }

            [Required]
            //[Required(ErrorMessage = "請輸入正確密碼")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "記住密碼")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {


            // 將已經登錄的用戶從登錄或註冊頁面重定向回主頁
            if (User.Identity.IsAuthenticated)
            {
                string userSession = HttpContext.Session.GetString("userId") ?? "Guest";
                if (userSession == "Guest")
                {
                    Response.Redirect("/Home");
                }
            }
            // ========================================
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

                //User query = await _context.Users.FirstOrDefaultAsync(m => m.UserId == users.UserId);

                //AspNetUser authquery = await _context.AspNetUsers.FirstOrDefaultAsync(a => a.Email == authuser.UserName);
                if (result.Succeeded)
                {
                    //HttpContext.Session.SetString("email", authquery.Email.ToString());
                    //HttpContext.Session.SetString("userName", authquery.UserName.ToString());
                    HttpContext.Session.SetString("userToastr", "已登入");
                    _logger.LogInformation("已登入");
                    return LocalRedirect(returnUrl);
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
                if (result.IsNotAllowed)
                {

                    return RedirectToPage("/Login");
                }
                else
                {
                    HttpContext.Session.SetString("userToastr", "帳號或密碼錯誤");
                    ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //額外做一個handler 存取 Session
        public async Task<IActionResult> OnPostLoginAsync(User users,AspNetUser authuser)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                AspNetUser query = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.UserName == users.Email);

                if (result.Succeeded)
                {
                    HttpContext.Session.SetString("userId", query.Id.ToString());
                    HttpContext.Session.SetString("userName", query.UserName.ToString());
                    //HttpContext.Session.SetString("userStatus", query.UserStatus.ToString());
                    HttpContext.Session.SetString("userToastr", "登入成功");

                    _logger.LogInformation("已登入");
                    return RedirectToPage("/Index");
                }
            }
            HttpContext.Session.SetString("userToastr", "帳號或密碼錯誤");
            return Redirect("/identity/Account/Login");
        }
    }
}
