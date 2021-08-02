using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Prj_CSharpGo.Areas.Identity.Data;
using Prj_CSharpGo.Models;

namespace Prj_CSharpGo.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<identityForUser> _userManager;
        private readonly SignInManager<identityForUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private WildnessCampingContext _context;

        public IndexModel(
            UserManager<identityForUser> userManager,
            SignInManager<identityForUser> signInManager,
            IEmailSender emailSender,
            WildnessCampingContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }

        [Display(Name = "帳號")]
        public string Username { get; set; }

        [Display(Name = "目前的電子郵件")]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }



        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public InputModelV1 Input1 { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "電話號碼")]
            public string PhoneNumber { get; set; }

        }

        public class InputModelV1
        {
            [Required]
            [EmailAddress]
            [Display(Name = "新的電子郵件")]
            public string NewEmail { get; set; }
        }
        private async Task LoadAsync(identityForUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            // Email

            Email = email;

            Input1 = new InputModelV1
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {

                //return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return RedirectToAction("/identity/Account/Login");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return RedirectToAction("/identity/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // PhoneNumber
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "發生未能預期的錯誤，請聯絡客服人員";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "您的會員檔案已變更";
            return RedirectToPage();
        }
    }
}
