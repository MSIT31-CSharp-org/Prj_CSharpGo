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
using SendGrid;

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

        // ===========================================================================================================================================================
        // 【GET】 ===================================================================================================================================================
        // ===========================================================================================================================================================

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

        // ===========================================================================================================================================================
        // 【POST】 ==================================================================================================================================================
        // ===========================================================================================================================================================

        [HttpPost]
        public IActionResult Login(string userId, string UserAccount, string UserPassword, string userStatus, string isSuccess)
        {
            if (!(string.IsNullOrEmpty(UserAccount) || string.IsNullOrEmpty(UserPassword)))
            {
                // 找出目前使用者欲登入的 UserId
                var userID = (from u in _context.Users
                              where u.UserAccount == UserAccount
                              //where u.UserPassword == UserPassword
                              select u.UserId).ToList()[0];
                var get_UserId = _context.Users.Find(userID);

                // UserId
                var f_userId = (from u in _context.Users
                                where u.UserAccount == UserAccount
                                select u.UserId).ToList();
                // UserStatus
                var f_userStatus = (from u in _context.Users
                                    where u.UserAccount == UserAccount
                                    select u.UserStatus).ToList();

                // 僅只取帳號
                var query = (from u in _context.Users
                             where u.UserAccount == UserAccount
                             select u).ToList();

                // 判斷帳號、密碼是否相等
                var queryList = (from u in _context.Users
                                 where u.UserAccount == UserAccount
                                 where u.UserPassword == UserPassword
                                 select u).ToList();

                // 取帳號認證狀態，判斷帳號、密碼是否相等
                var ifSuccess = (from u in _context.Users
                                 where u.UserAccount == UserAccount
                                 where u.UserPassword == UserPassword
                                 select u.IsSuccess).ToList();
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // 帳號存在 ?
                int x = query.Count;
                // 帳號、密碼存在 ?
                int s = queryList.Count;
                // 帳號通過Email驗證 ?
                int y = ifSuccess.Count;
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


                if (get_UserId.UserStatus.ToString() != "NR")
                {
                    if (get_UserId.UserStatus.ToString() == "SP")
                    {
                        HttpContext.Session.SetString("userToastr", "目前此帳號狀態已遭停權，如有任何帳號疑慮，敬請聯繫官方服務中心");
                        //return Redirect("/Home/Login");
                        return View();
                    }
                    else if (get_UserId.UserStatus.ToString() == "WL")
                    {
                        HttpContext.Session.SetString("userToastr", "目前此帳號狀態已被註銷，如有任何帳號疑慮，敬請聯繫官方服務中心");
                        //return Redirect("/Home/Login");
                        return View();
                    }
                }
                // 登入成功  寫入Session
                else if (s == 1 && y == 1 && get_UserId.IsSuccess == true)
                {
                    HttpContext.Session.SetString("userToastr", "登入成功");
                    HttpContext.Session.SetString("userId", f_userId[0].ToString());
                    HttpContext.Session.SetString("userStatus", f_userStatus[0].ToString());
                    HttpContext.Session.SetString("userIsSuccess", ifSuccess[0].ToString());
                    //HttpContext.Session.SetString("userAccount", userAccount);
                    //HttpContext.Session.SetString("userPassword", userPassword);
                    //return Content("登入成功");
                    return Redirect("/Auth/Index");
                }
                else if (x == 0)
                {
                    HttpContext.Session.SetString("userToastr", "此帳號不存在");
                    //return Content("無法使用此帳號登入");
                    return View();
                }
                else if (y == 0)
                {
                    HttpContext.Session.SetString("userToastr", "您的帳號未開通");
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


            // ~~~~~~~~~~~~ 取得用戶帳號 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var ACQuery = from o in _context.Users
                          where o.UserAccount == account
                          select o;
            // ~~~~~~~~~~~~ 取得用戶Email ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var EmailQuery = from o in _context.Users
                             where o.Email == email
                             select o;
            // ~~~~~~~~~~~~ 取得主密碼 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var PwdQuery = from o in _context.Users
                           where o.UserPassword == password
                           select o;
            // ~~~~~~~~~~~~ 取得第二道驗證密碼 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var ConfirmPwdQuery = from o in _context.Users
                                  where o.ConfirmPassword == confirmPassword
                                  select o;
            // ~~~~~~~~~~~~ 取得帳號目前狀態 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var UpdateStatus = from o in _context.Users
                               where o.UserStatus == userStatus
                               select o;


            // 1. ------------------- 判斷帳號是否已被創建 -------------------
            if (ACQuery.Count() != 0)
            {
                HttpContext.Session.SetString("userToastr", "此帳號已有人使用！");
                //return Content("此帳號已有人使用！");
                return View();
            }

            // 2. ------------------- 判斷輸入密碼格式 ----------------------
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                HttpContext.Session.SetString("userToastr", "密碼格式有誤");
                //return Content("密碼格式有誤");
                return View();
            }

            // 3. ------------------- 判斷前後密碼相符 ----------------------
            if (password != confirmPassword)
            {
                HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                //return Content("前後密碼不相符");
                return View();
            }

            // 4. ------------------- 判斷信箱是否已被其他用戶使用 ----------------------
            if (EmailQuery.Count() == 1)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    HttpContext.Session.SetString("userToastr", "Email格式有誤");
                    //return Content("Email格式有誤");
                    return View();
                }
                HttpContext.Session.SetString("userToastr", "已被使用的Email");
                //return Content("已被使用的Email");
                return View();

            }
            HttpContext.Session.SetString("userToastr", "是您可私有的Email");

            // 5. ------------------- 判斷用戶帳號使用權(還沒創建前都是Null) ----------------------

            if (userStatus != null)
            {
                if (userStatus == "SP")
                {
                    HttpContext.Session.SetString("userToastr", "目前此帳號狀態已遭停權，如有任何帳號疑慮，敬請聯繫官方服務中心");
                    return Redirect("/Home/Index");
                }
                else if (userStatus == "WL")
                {
                    HttpContext.Session.SetString("userToastr", "目前此帳號狀態已被註銷，如有任何帳號疑慮，敬請聯繫官方服務中心");
                    return Redirect("/Home/Index");
                }
                return Redirect("/Auth/Login");
            }

            // *********************************************************************************************************
            // *********************************************************************************************************
            // **********************【 完成以上判斷條件式，若是可以註冊，就執行以下程式碼片段 】****************************
            // *********************************************************************************************************            
            // *********************************************************************************************************

            User newMember = new User
            {
                //UserId = userid,      // PK自動編號
                Email = email,
                UserAccount = account,
                UserPassword = password,
                ConfirmPassword = confirmPassword,
                UserName = "",
                UserStatus = userStatus,
                UpdateDate = DateTime.Now,
                IsSuccess = false
            };
            _context.Users.Add(newMember);
            _context.SaveChanges();

            var accountEncode = Encoding.UTF8.GetBytes(account);
            var validAccountEncode = WebEncoders.Base64UrlEncode(accountEncode);

            string url = $"{_configuration["AppUrl"]}/Auth/confirmEmail?id={account}&token={validAccountEncode}";
            await _mailService.SendEmailAsync(email,
                            "做夥暢行露營會員資格吧！",
                            $"<h1><i>Welcome to WildnessCamping</i></h1>" +
                            $"<h3 class='font-weight-bold'>請點擊下方按鈕以完成會員資格開通</h3>" +
                            $"<h3 class='font-weight-bold'><a href='{url}' class='btn btn-primary stretched-link' style='text-decoration:none;'>完成驗證</a></h3>");

            HttpContext.Session.SetString("userToastr", "您現在可以至電子郵件收取會員註冊信以完成驗證程序");
            HttpContext.Session.SetString("userId", userid.ToString());
            HttpContext.Session.SetString("userEmail", email);
            HttpContext.Session.SetString("userPassword", password);
            HttpContext.Session.SetString("userIsSuccess", isSuccess.ToString());
            //HttpContext.Session.SetString("userStatus", userStatus.ToString());
            //HttpContext.Session.SetString("userName", username);

            //return Content("註冊成功");
            //return RedirectToAction("Login", "Auth");
            return View();
        }


        // Email驗證 => 使用者點擊返回驗證模組
        public IActionResult confirmEmail()
        {
            string account = HttpContext.Request.Query["id"].ToString();
            string token = HttpContext.Request.Query["token"].ToString();

            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var userID = (from u in _context.Users
                          where u.UserAccount == normalToken
                          select u.UserId).ToList()[0];

            var changUserStatus = _context.Users.Find(userID);
            changUserStatus.IsSuccess = true;   // IsSuccess 型態為 bool
            changUserStatus.UserStatus = "NR";
            changUserStatus.UpdateDate = DateTime.Now;

            HttpContext.Session.SetString("userToastr", "會員已開通完成");
            HttpContext.Session.SetString("userStatus", changUserStatus.UserStatus);
            HttpContext.Session.SetString("userIsSuccess", changUserStatus.IsSuccess.ToString());
            HttpContext.Session.SetString("userUpadteDate", changUserStatus.UpdateDate.ToString());
            //HttpContext.Session.SetString("userId", changUserStatus.UserId.ToString());
            //HttpContext.Session.SetString("userEmail", changUserStatus.Email);
            //HttpContext.Session.SetString("userAccount", changUserStatus.UserAccount);
            //HttpContext.Session.SetString("userPassword", changUserStatus.UserPassword);

            _context.SaveChanges();
            return Redirect("/Auth/Login");
            //return RedirectToAction("Login", "Auth");
        }

    }
}

