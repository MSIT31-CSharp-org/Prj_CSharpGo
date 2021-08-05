using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.ViewModels;
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

        // 【GET】 ===================================================================================================================================================

        // 登入
        public IActionResult Login()
        {
            return View();
        }

        // 註冊
        public IActionResult Register()
        {
            //string userSession = HttpContext.Session.GetString("userId") ?? "Guest";
            //if (userSession == "Guest")
            //{
            //    return Redirect("/Auth/Login");
            //}
            return View();
        }

        // 登出
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.SetString("userToastr", "已成功登出");
            return Redirect("/Home/Index");
        }

        // 會員中心
        public IActionResult Index(int? id)
        {
            //List<User> userList = _context.Users.ToList();


            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            //HttpContext.Session.SetString("userToastr", "登入成功");

            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            var userInfo = _context.Users.Find(id);
            return View(userInfo);

        }


        // 【POST】 ===================================================================================================================================================

        [HttpPost]
        public IActionResult Login(string userId, string UserAccount, string UserPassword, string userStatus, bool isSuccess)
        {
            //userName = _context.Users.Where(e => e.UserName == userName).ToString();

            if (!(string.IsNullOrEmpty(UserAccount) || string.IsNullOrEmpty(UserPassword)))
            {
                // UserId
                var f_userId = (from u in _context.Users
                                where u.UserAccount == UserAccount
                                select u.UserId).ToList();
                // UserStatus
                var f_userStatus = (from u in _context.Users
                                    where u.UserAccount == UserAccount
                                    select u.UserStatus).ToList();


                var query = (from u in _context.Users
                             where u.UserAccount == UserAccount
                             select u).ToList();

                var queryList = (from u in _context.Users
                                 where u.UserAccount == UserAccount
                                 where u.UserPassword == UserPassword
                                 select u).ToList();

                var ifSuccess = (from u in _context.Users
                                 where u.IsSuccess == isSuccess
                                 select u).ToList();

                int x = query.Count;
                int s = queryList.Count;
                int y = ifSuccess.Count;

                // 登入成功  寫入Session
                if (s == 1 && y == 1)
                {
                    HttpContext.Session.SetString("userToastr", "登入成功");
                    HttpContext.Session.SetString("userId", f_userId[0].ToString());
                    //HttpContext.Session.SetString("userAccount", userAccount);
                    //HttpContext.Session.SetString("userPassword", userPassword);
                    HttpContext.Session.SetString("userStatus", f_userStatus[0].ToString());
                    //HttpContext.Session.SetString("userIsSuccess", isSuccess.ToString());
                    //return Content("登入成功");
                    return Redirect("/Auth/Index");
                }
                else if (x == 0)
                {
                    HttpContext.Session.SetString("userToastr", "不存在此帳號");
                    //return Content("無法使用此帳號登入");
                    return View();
                }
                else if (x != 0 && y == 0)
                {
                    HttpContext.Session.SetString("userToastr", "帳號尚未開通，請至電子信箱確認");
                    //return Content("帳號尚未開通，請至電子郵件收信");
                    return View();
                }
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Register(int userid, string account, string password, string confirmPassword, string email, string username, string userStatus, bool isSuccess)
        {
            // 【正則表達式 Regex】 -------------------------------------------------------------------------------------
            Regex regexAccount = new Regex(@"^[a-zA-Z0-9]{6,20}$");
            Regex regexPassword = new Regex(@"^(?!.*[^\x21-\x7e])(?=.{6,20})(?=.*[a-zA-Z])(?=.*[a-zA-Z])(?=.*\d).*$");
            // ------------------- -------------------------------------------------------------------------------------

            var ACQuery = from o in _context.Users
                          where o.UserAccount == account
                          select o;

            var EmailQuery = from o in _context.Users
                             where o.Email == email
                             select o;

            var PwdQuery = from o in _context.Users
                           where o.UserPassword == password
                           select o;

            var ConfirmPwdQuery = from o in _context.Users
                                  where o.ConfirmPassword == confirmPassword
                                  select o;


            // if (String.IsNullOrEmpty(account) || !(regexAccount.IsMatch(account))){
            //    if (ACQuery.Count() != 0)
            //    {
            //        HttpContext.Session.SetString("userToastr", "此帳號已有人使用！"); return Content("此帳號已有人使用！");
            //        return View();
            //    }
            //    else if (ACQuery.Count() == 0) { return Content("可由您建立的帳號！"); return Redirect("/Auth/confirmemail"); }
            //    HttpContext.Session.SetString("userToastr", "帳號格式有誤"); return Content("帳號格式有誤");
            //  }

            // ------------------- 判斷帳號是否已被創建 -------------------
            if (ACQuery.Count() != 0)
            {
                HttpContext.Session.SetString("userToastr", "此帳號已有人使用！");
                //return Content("此帳號已有人使用！");
                return View();
            }

            // ------------------- 判斷前後密碼相符 ----------------------
            if (password != confirmPassword)
            {
                HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                //return Content("前後密碼不相符");
                return View();
            }

            // ------------------- 判斷輸入密碼格式 ----------------------
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                HttpContext.Session.SetString("userToastr", "密碼格式有誤");
                //return Content("密碼格式有誤");
                return View();
            }

            // ------------------- 判斷信箱是否已被其他用戶使用 ----------------------
            if (EmailQuery.Count() == 1)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    HttpContext.Session.SetString("userToastr", "Email格式有誤");
                    return Content("Email格式有誤");
                }
                HttpContext.Session.SetString("userToastr", "已被使用的Email");
                return Content("已被使用的Email");

            }
            HttpContext.Session.SetString("userToastr", "是您可私有的Email");


            User newMember = new User
            {
                //UserId = userid,      // PK自動編號
                Email = email,
                UserAccount = account,
                UserPassword = password,
                ConfirmPassword = confirmPassword,
                UserName = username,
                UserStatus = "NR",
                UpdateDate = DateTime.Now,
                IsSuccess = isSuccess
            };
            _context.Users.Add(newMember);
            _context.SaveChanges();

            var accountEncode = Encoding.UTF8.GetBytes(account);
            var validAccountEncode = WebEncoders.Base64UrlEncode(accountEncode);

            string url = $"{_configuration["AppUrl"]}/Auth/confirmEmail?id={account}&token={validAccountEncode}";
            await _mailService.SendEmailAsync(email, "一起露營吧！", $"<h1>Welcome to WildnessCamping</h1>" +
                            $"<p>Please confirm your email by <a href='{url}'>點擊這裡</a></p>");

            HttpContext.Session.SetString("userToastr", "您現在可以至電子郵件收取會員註冊信以完成驗證程序");
            HttpContext.Session.SetString("userId", userid.ToString());
            HttpContext.Session.SetString("userEmail", email);
            HttpContext.Session.SetString("userName", username);
            HttpContext.Session.SetString("userPassword", password);
            HttpContext.Session.SetString("userStatus", userStatus);
            HttpContext.Session.SetString("userIsSuccess", isSuccess.ToString());

            return Redirect("/Auth/confirmemail");
            //return Content("註冊成功");

        }



        // Email驗證 => 使用者點擊返回驗證模組
        public IActionResult confirmEmail()
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
                          where u.UserAccount.ToString() == normalToken
                          select u.UserId).ToList()[0];

            var changUserStatus = _context.Users.Find(userID);
            changUserStatus.IsSuccess = true;   // IsSuccess 型態為 bool
            changUserStatus.UpdateDate = DateTime.Now;


            HttpContext.Session.SetString("userId", changUserStatus.UserId.ToString());
            HttpContext.Session.SetString("userEmail", changUserStatus.Email);
            HttpContext.Session.SetString("userAccount", changUserStatus.UserAccount);
            HttpContext.Session.SetString("userPassword", changUserStatus.UserPassword);
            HttpContext.Session.SetString("userStatus", changUserStatus.UserStatus);
            HttpContext.Session.SetString("userIsSuccess", changUserStatus.IsSuccess.ToString());
            HttpContext.Session.SetString("userIsSuccess", changUserStatus.UpdateDate.ToString());
            HttpContext.Session.SetString("userToastr", "會員已開通完成");
            _context.SaveChanges();
            return Redirect("/Auth/Index");
        }

        //public class CheckAccess
        //{
        //    public int? Account { get; set; }
        //    public int? Email { get; set; }
        //    public int? UserName { get; set; }
        //    public string password { get; set; }

        //    [Display(Name = "記住密碼")]
        //    public bool RememberMe { get; set; }

        //    [TempData]
        //    public string ErrorMessage { get; set; }
        //    public IEnumerable<User> userList { get; set; }
        //}










    }



}

