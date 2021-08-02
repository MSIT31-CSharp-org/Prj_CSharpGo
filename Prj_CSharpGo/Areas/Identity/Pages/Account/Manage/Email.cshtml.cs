using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Prj_CSharpGo.Areas.Identity.Data;

namespace Prj_CSharpGo.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<identityForUser> _userManager;
        private readonly SignInManager<identityForUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<identityForUser> userManager,
            SignInManager<identityForUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            // 判斷要變更的Email是否已存在，表示有其他使用者已經註冊使用
            var Existuser = await _userManager.FindByEmailAsync(Input1.NewEmail);
            // 判斷要變更的Email和舊的Email(同帳號)是否相同
            var oldemail = await _userManager.GetUserAsync(User);

            if (Existuser != null)
            {

                if (Existuser != null && Existuser == oldemail)
                {
                    StatusMessage = "電子郵件未變動！";
                    return RedirectToPage();
                }

                StatusMessage = "此電子郵件已有其他使用者註冊使用！";
                return RedirectToPage();
            }


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

            var email = await _userManager.GetEmailAsync(user);
            if (Input1.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input1.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input1.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input1.NewEmail,
                    "請至電子信箱確認",
                    $"<h1><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處鏈結完成電子郵件驗證</a></h1>");

                StatusMessage = "已寄出驗證信至您變更後的電子信箱，請至該電子信箱確認";
                return RedirectToPage();
            }

            StatusMessage = "很抱歉，您的Email未變更";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"<h1><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>點擊此處鏈結完成電子郵件驗證</a></h1>");

            StatusMessage = "已寄出驗證信，請至電子信箱確認";
            return RedirectToPage();
        }
    }
}
