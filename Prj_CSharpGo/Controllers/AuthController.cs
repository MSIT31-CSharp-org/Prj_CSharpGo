using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.identity;
using Prj_CSharpGo.Services;

namespace Prj_CSharpGo.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<CampController> _logger;
        private WildnessCampingContext _context;
        private IConfiguration _configuration;
        private IMailService _mailService;

        public AuthController(ILogger<CampController> logger, WildnessCampingContext context, IMailService mailService, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _mailService = mailService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }


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

            [Display(Name = "記住密碼")]
            public bool RememberMe { get; set; }
        }




        // GET: Auth
        // 會員中心介面
        public IActionResult Index(int? id)
        {
            //List<User> userList = _context.Users.ToList();

            User userInfo = _context.Users.Find(id);
            string userSession = HttpContext.Session.GetString("userId") ?? "Guest";
            HttpContext.Session.SetString("userToastr", "登入成功");

            if (userSession == "Guest")
            {
                return Redirect("/Auth/Login");
            }
            return View(userInfo);

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            //if (user == null)
            //{
            //    return NotFound();
            //}
            //return View(user);
        }

        // ===================================================================================================================================================


        // 登入  GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user, string email, string userPassword)
        {
            //if (string.IsNullOrEmpty(email))
            //{
            //    return View();
            //}

            HttpContext.Session.SetString("userId", email);
            return Redirect("/Home/Index");


            // 驗證帳密
            var userLogin = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);

            if (ModelState.IsValid)
            {
                if (userLogin != null && userLogin.UserPassword == user.UserPassword)
                {
                    if (userLogin.Email == null)
                    {
                        HttpContext.Session.SetString("userToastr", "帳號不存在");
                        return Redirect("/Auth/Login");
                    }

                    if (userLogin.IsSuccess == false)
                    {
                        HttpContext.Session.SetString("userToastr", "帳號尚未開通，請至電子郵件收信");
                        return Redirect("/Auth/Login");
                    }

                    // 登入成功  寫入Session
                    HttpContext.Session.SetString("userToastr", "登入成功");
                    HttpContext.Session.SetString("userId", userLogin.UserId.ToString());
                    HttpContext.Session.SetString("userName", userLogin.UserName.ToString());
                    HttpContext.Session.SetString("userStatus", userLogin.UserStatus.ToString());
                    HttpContext.Session.SetString("userIsSuccess", userLogin.IsSuccess.ToString());
                    return Redirect("/Auth/Index");
                }
                return Redirect("/Auth/Index");
            }
            HttpContext.Session.SetString("userToastr", "帳號或密碼錯誤，請重新輸入");
            return View();
            //return Redirect("/Auth/Login");
        }




        // 註冊  GET: /Auth/Login
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {

            //User userConfirm = new User()
            //{
            //    //UserId = 0,
            //    Email = "",
            //    UserPassword = "",
            //    ConfirmPassword = "",
            //    //UserStatus = "SP",   // 帳號尚未開通 = "SP"
            //    //IsSuccess = false    // 啟用帳號 = false
            //};

            if (user == null)
                throw new NullReferenceException("很抱歉，目前無法使用註冊功能");

            if (ModelState.IsValid)
            {
                // 判斷前後密碼相符
                if (user.UserPassword != user.ConfirmPassword)
                {
                    HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                    return View();
                }
                // 判斷UserId 存不存在
                if (await _context.Users.FindAsync(user.UserId) != null)
                {
                    HttpContext.Session.SetString("userToastr", "帳號已存在");
                    return View();
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
            };
            HttpContext.Session.SetString("userToastr", "您現在可以至電子郵件收取會員註冊信以完成驗證程序");
            return Redirect("/Auth/Index");
        }

        // 登出 Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.SetString("userToastr", "登出成功");
            return Redirect("/Auth/Login");
        }


        // Email => 寄送驗證信
        public IActionResult SendConfirmEmail()
        {
            return Redirect("");
        }



        // Email => 使用者點擊返回驗證模組
        public IActionResult ConfirmEmail()
        {
            string account = HttpContext.Request.Query["Id"].ToString();
            string token = HttpContext.Request.Query["token"].ToString();
            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var userID = (from u in _context.Users
                          where u.Email == normalToken
                          select u.UserId).ToList()[0];
            var changUserStatus = _context.Users.Find(userID);
            changUserStatus.UserStatus = "NR";
            changUserStatus.IsSuccess = true;   // IsSuccess 型態為 bool
            changUserStatus.UpdateDate = DateTime.Now;

            HttpContext.Session.SetString("userToastr", "會員已開通完成，現在請放心購物");
            _context.SaveChanges();
            return View();

        }


    }



}

