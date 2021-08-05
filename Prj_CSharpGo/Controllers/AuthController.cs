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

        // GET: Auth
        // 會員中心介面
        public IActionResult Index(int? id)
        {
            //List<User> userList = _context.Users.ToList();


            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            HttpContext.Session.SetString("userToastr", "登入成功");

            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            var userInfo = _context.Users.Find(id);
            return View(userInfo);

        }

        // ===================================================================================================================================================


        // 登入  GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Login(User user,string email, string userPassword)
        {
            User query = _context.Users.Where(m => m.Email == user.Email).FirstOrDefault();
            //var member = _context.Users.Where(m => m.Email == email && m.UserPassword == userPassword).FirstOrDefault();

            if (query != null && query.UserPassword == user.UserPassword)
            {
                HttpContext.Session.SetString("userToastr", "帳號不存在");
                return View();
            }
            if (query.IsSuccess == false)
            {
                HttpContext.Session.SetString("userToastr", "帳號尚未開通，請至電子郵件收信");
                return View();
            }
            // 登入成功  寫入Session
            HttpContext.Session.SetString("userToastr", "登入成功");
            HttpContext.Session.SetString("userId", query.UserId.ToString());
            HttpContext.Session.SetString("userIsSuccess", query.IsSuccess.ToString());
            return RedirectToAction("Index");
        }




        // 註冊  GET: /Auth/Login
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user,IdentityUser userid)
        {
            var userConfirm = new User()
            {
                UserId = user.UserId,
                Email = user.Email,
                UserPassword = user.UserPassword,
                ConfirmPassword = user.ConfirmPassword,
                UserStatus = user.UserStatus,   // 帳號尚未開通 = "SP"
                IsSuccess = user.IsSuccess    // 啟用帳號 = false
            };

            if (ModelState.IsValid == false)
            {
                return View();
            }
            // 判斷前後密碼相符
            if (user.UserPassword != user.ConfirmPassword)
            {
                HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                user.IsSuccess = false;
                return View();
            }
            //var tmp = _context.Users.Find(user.Email.ToString());

            var userExist = (from u in _context.Users
                          where u.Email == user.Email
                          select u.Email).ToList()[0];


            // 判斷 Email 存不存在
            if (userExist != null)
            {
                HttpContext.Session.SetString("userToastr", "很抱歉，您無法使用此組信箱建立帳號");
                return View();
            }

            var member = _context.Users.Where(m => m.Email == user.Email).FirstOrDefault();

            if (member.IsSuccess == null)
            {
                var confirmEmailToken = _context.Users.Where(m => m.UserId == user.UserId);
                var encodedEmailToken = Encoding.UTF8.GetBytes((confirmEmailToken).ToString());
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={user.UserId}&token={validEmailToken}";

                _mailService.SendEmailAsync(user.Email, "Confirm your email", $"<h1>Welcome to Auth Demo</h1>" +
                        $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

                HttpContext.Session.SetString("userToastr", "您現在可以至電子郵件收取會員註冊信以完成驗證程序");
                HttpContext.Session.SetString("userStatus", user.UserStatus.ToString());
                HttpContext.Session.SetString("userIsSuccess", user.IsSuccess.ToString());

                var exchangeserStatus = _context.Users.Find(user);
                exchangeserStatus.UserStatus = "NR";
                exchangeserStatus.IsSuccess = false;   // IsSuccess 型態為 bool
                _context.Add(user);
                _context.SaveChanges();
                return Redirect("/Auth/Login");
            }
            return View();

        }



        // Email => 寄送驗證信
        [HttpPost]
        public IActionResult SendConfirmEmail(User user)
        {
            return NotFound();
        }



        // Email => 使用者點擊返回驗證模組
        [HttpGet("ConfirmEmail")]
        public IActionResult ConfirmEmail()
        {
            string id = HttpContext.Request.Query["Id"].ToString();
            string token = HttpContext.Request.Query["token"].ToString();

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var userID = (from u in _context.Users
                          where u.UserId.ToString() == normalToken
                          select u.UserId).ToList()[0];

            var changUserStatus = _context.Users.Find(userID);
            changUserStatus.UserStatus = "NR";
            changUserStatus.IsSuccess = true;   // IsSuccess 型態為 bool
            changUserStatus.UpdateDate = DateTime.Now;

            HttpContext.Session.SetString("userToastr", "會員已開通完成，現在請放心購物");
            _context.SaveChanges();
            return View();

        }



        // 登出 Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.SetString("userToastr", "登出成功");
            return Redirect("/Auth/Login");
        }


    }



}

