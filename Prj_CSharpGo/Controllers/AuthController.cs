using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        // =================================================================================================================================================================================
        // 【GET】 =========================================================================================================================================================================
        // =================================================================================================================================================================================

        // 登入
        public IActionResult Login()
        {
            return View();
        }

        // 註冊
        public IActionResult Register()
        {
            return View();
        }

        // 登出
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("userId");
            HttpContext.Session.SetString("userToastr", "已成功登出");

            // 登出成功後，等待1.0秒轉導回首頁
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ==>") + ex.ToString());
            }
            return View("LogoutTurn");
        }

        // 登出轉導頁
        public IActionResult LogoutTurn()
        {
            return View();
        }

        // 會員中心
        public IActionResult Index()
        {
            // 接收 Login Form 傳入的 Session
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 判斷傳入的 userId ，身分若是訪客，就將使用者導回登入頁
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u).FirstOrDefault();

            return View("Index", userID);
        }

        // 會員中心 => 密碼變更
        public IActionResult ChangePassword()
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 判斷 Session 傳入的 userId ，身分若是訪客，就將使用者導回登入頁
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u).FirstOrDefault();

            return View(userID);
        }

        // =================================================================================================================================================================================
        // 【POST : 登入 / 註冊】 ===========================================================================================================================================================
        // =================================================================================================================================================================================

        // 登入
        [HttpPost]
        public IActionResult Login(string UserAccount, string UserPassword, string userStatus)
        {
            if (!(string.IsNullOrEmpty(UserAccount) || string.IsNullOrEmpty(UserPassword)))
            {
                // UserID
                var f_userId = (from u in _context.Users
                                where u.UserAccount == UserAccount
                                select u.UserId).ToList();

                // 判斷若當即登入的該使用者帳號是否存在
                if (f_userId.Count() == 0)
                {
                    HttpContext.Session.SetString("userToastr", "此帳號不存在");
                    return View();
                }


                // 僅只取帳號
                var queryAccount = (from u in _context.Users
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

                // UserStatus
                var NRStatus = (from u in _context.Users
                                where u.UserStatus == userStatus
                                select u).ToList();

                // 找出目前使用者欲登入的 UserId
                var get_UserId = _context.Users.Find(f_userId[0]);

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // 帳號存在 ?
                int x = queryAccount.Count;
                // 帳號、密碼存在 ?
                int s = queryList.Count;
                // 帳號通過Email驗證 ?
                int y = ifSuccess.Count;
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


                if (get_UserId.UserStatus.ToString() != "NR" || get_UserId.UserStatus.ToString() == null)
                {
                    if (get_UserId.UserStatus.ToString() == "SP")
                    {
                        HttpContext.Session.SetString("userToastr", "目前此帳號狀態已遭停權，如有任何帳號疑慮，敬請聯繫官方服務中心");
                        //return Redirect("/Home/Login");
                        return View();
                    }
                    else if (get_UserId.UserStatus.ToString() == "WL")
                    {
                        HttpContext.Session.SetString("userToastr", "此帳號不存在");
                        //return Redirect("/Home/Login");
                        return View();
                    }
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
                // 登入成功  寫入Session
                else if (s == 1 && y == 1 && get_UserId.IsSuccess == true)
                {
                    HttpContext.Session.SetString("userToastr", "登入成功");
                    HttpContext.Session.SetString("userId", get_UserId.UserId.ToString());
                    return Redirect("/Auth/Index");
                }

            }
            return View();

        }

        // 註冊
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
                    HttpContext.Session.SetString("userToastr", "輸入的Email格式有誤");
                    //return Content("Email格式有誤");
                    return View();
                }
                HttpContext.Session.SetString("userToastr", "此Email已被使用");
                //return Content("已被使用的Email");
                return View();

            }
            HttpContext.Session.SetString("userToastr", "可用的Email");

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
                    HttpContext.Session.SetString("userToastr", "此帳號不存在");
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
                UserStatus = "NR",
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


            HttpContext.Session.SetString("userToastr", $"請至您的電子郵件 {email} 開通會員資格認證信");

            // 註冊成功後，等待1.0秒轉導至登入頁
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (TaskCanceledException ex)
            {
                string Status = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ==>") + ex.ToString();
                HttpContext.Session.SetString("userToastr", Status);
                return View();
            }

            return View("Login");
        }

        // =================================================================================================================================================================================
        // 【POST : 會員中心 】 =============================================================================================================================================================
        // =================================================================================================================================================================================

        // 會員中心 => 基本資料變更
        [HttpPost]
        public async Task<IActionResult> MemberInfo(string email,string username, string phone,DateTime birthday, string region, string address)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u.UserId).FirstOrDefault();

            // 優先判斷輸入的資訊舊密碼是否存在
            if (userID.ToString() != userId )
            {
                HttpContext.Session.SetString("userToastr", "請重新輸入您的會員基本資料");
                return Redirect("/Auth/Index");
            }

            var uBirth = (from u in _context.Users
                          where u.Birthday.ToString() == birthday.ToShortDateString()
                          select u).FirstOrDefault();

            // 設定該使用者之 UserId 用以變更
            var changMemberinfo = _context.Users.Find(userID);
            changMemberinfo.UserName = username;
            changMemberinfo.Phone = phone;
            changMemberinfo.Birthday = birthday;
            changMemberinfo.Region = region;
            changMemberinfo.Address = address;
            changMemberinfo.UpdateDate = DateTime.Now;

            _context.Update(changMemberinfo);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("userToastr", "您的會員資料變更成功");

            return View("Index");
        }

        // 會員中心 => 密碼變更
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string NewPassword, string confirmNewPassword)
        {
            // 背景作業 ------ 取得舊密碼
            var f_userId = (from u in _context.Users
                            where u.UserPassword == password
                            select u.UserId).ToList();
            // 優先判斷輸入的舊密碼是否存在
            if (f_userId.Count() != 1)
            {
                HttpContext.Session.SetString("userToastr", "您輸入的舊密碼有誤");
                return View();
            }

            // 背景作業 ------ 找出目前使用者的 UserId
            var get_UserId = _context.Users.Find(f_userId[0]);

            // ------------------- 判斷輸入舊密碼格式 ----------------------
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                // 比對輸入的舊密碼 == 背景取得的現今密碼 
                if (password != get_UserId.UserPassword)
                {
                    HttpContext.Session.SetString("userToastr", "您輸入的舊密碼有誤");
                    return View();
                }
                // 比對輸入的新密碼 == 二道驗證新密碼
                else if (NewPassword != confirmNewPassword)
                {
                    HttpContext.Session.SetString("userToastr", "新密碼前後密碼不相符");
                    return View();
                }
                // 新密碼不可與舊密碼相同
                else if (password == NewPassword && password == confirmNewPassword)
                {
                    HttpContext.Session.SetString("userToastr", "新密碼不可與舊密碼相同");
                    return View();
                }
                // 密碼空空如也
                else if (password == null || NewPassword == null || confirmNewPassword == null)
                {
                    HttpContext.Session.SetString("userToastr", "密碼空空如也");
                    return View();
                }
                // 密碼空空如也
                else if (password == null && NewPassword == null && confirmNewPassword == null)
                {
                    HttpContext.Session.SetString("userToastr", "密碼空空如也");
                    return View();
                }
            }

            // 變更密碼成功，等待1.0秒轉導至登入頁
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (TaskCanceledException ex)
            {
                string Status = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ==>") + ex.ToString();
                HttpContext.Session.SetString("userToastr", "很抱歉！密碼未能變更成功");
                return View();
            }

            User member = new User()
            {
                UserPassword = NewPassword,
                ConfirmPassword = NewPassword
            };

            var changUserNewPwd = _context.Users.Find(f_userId);
            changUserNewPwd.UserPassword = NewPassword;
            changUserNewPwd.ConfirmPassword = confirmNewPassword;

            _context.Update(member);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("userToastr", "密碼變更成功，請重新登入");
            return Redirect("/Auth/Login");
        }

        // =================================================================================================================================================================================
        // 【POST : 寄信驗證】 ==============================================================================================================================================================
        // =================================================================================================================================================================================

        // 寄發會員資格開通驗證信
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

            _context.SaveChanges();
            return Redirect("/Auth/Login");
        }


    }
}

