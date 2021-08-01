using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Areas.Identity.Data;
namespace Prj_CSharpGo.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<identityForUser> _userManager;
        private readonly SignInManager<identityForUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<identityForUser> userManager,
            SignInManager<identityForUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "必要欄位")]
            [DataType(DataType.Password)]
            [Display(Name = "請輸入舊密碼")]
            //[Compare("Password", ErrorMessage = "與舊密碼不一致")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "必要欄位")]
            [StringLength(100, ErrorMessage = "密碼請輸入至少{1}位英數字", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "新密碼")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "確認新密碼")]
            [Compare("NewPassword", ErrorMessage = "新密碼前後不一致")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("使用者已成功變更密碼");
            StatusMessage = "已成功變更密碼";

            return RedirectToPage();
        }
    }
}
