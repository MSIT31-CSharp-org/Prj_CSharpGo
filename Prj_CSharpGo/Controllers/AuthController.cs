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
        // 【GET : 寄信驗證】 ==============================================================================================================================================================
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

            // 登出成功後，等待0.5秒轉導回首頁
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(500, cts.Token);
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


        // 會員中心 => 訂單資料顯示
        public IActionResult MemberOrder()
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u.UserId).FirstOrDefault();

            if (userId == "Guest" || userID.ToString() != userId || userId == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }

            // 找出此會員的所有訂單
            var f_UserID = _context.Users.Find(userID);

            // ·························································································
            // ························ 兩種方法 : 找到此 UserId 的所有訂單 ······························
            // ····················· 10 ································· 10 ···························
            var MemberOrderView = _context.Orders.Where(o => o.UserId == f_UserID.UserId).ToList();
            // ····················· 20 ································· 20 ···························
            //var MOrder = (from u in _context.Orders
            //              where u.UserId == f_UserID.UserId
            //              select u).ToList();
            // ·························································································

            if (MemberOrderView == null)
            {
                HttpContext.Session.SetString("userToastr", "已為您查詢訂單，但未能成功");
                return View();
            }
            return View(MemberOrderView);
        }

        // 會員中心 => 訂單變更
        public async Task<IActionResult> MemberOrderEdit(int? id)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = _context.Users.Where(u => u.UserId.ToString() == userId).FirstOrDefault();

            // 判斷 Session 傳入的 userId 身分是否為訪客
            if (userId == "Guest" || userID.UserId.ToString() != userId || userId == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }

            var MemberOrderDetailView = _context.OrderDetails.Where(o => o.OrderId == id).ToList()[0];

            var CancelOrder = (from u in _context.OrderDetails
                               where u.OrderId == MemberOrderDetailView.OrderId
                               select u.OrderId).ToList()[0];
            var f_order_user = (from u in _context.Users
                                where u.UserId == userID.UserId
                                select u.UserId).ToList()[0];

            // 藉由找出訂單ID 列出詳細訂單資訊
            if (MemberOrderDetailView == null)
            {
                HttpContext.Session.SetString("userToastr", " SoS！顯示異常");
                return View();
            }

            MemberOrder User_order = new MemberOrder()
            {
                _order = await _context.Orders.FindAsync(id),
                OrderDetails = _context.OrderDetails.Where(o => o.OrderId == id),
                Products = await _context.Products.ToListAsync(),
                //Users = _context.Users.Where(u => u.UserId == f_order_user)
            };

            if (User_order == null)
            {
                return NotFound();
            }

            return View(User_order);

        }



        // =================================================================================================================================================================================
        // 【POST : 登入 / 註冊】 ===========================================================================================================================================================
        // =================================================================================================================================================================================

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++【 登入 】+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        public IActionResult Login(string UserAccount, string UserPassword)
        {
            // 先判斷登入需要輸入的欄位是否有空值或是回傳Null的情形
            if (!(string.IsNullOrEmpty(UserAccount) || string.IsNullOrEmpty(UserPassword)))
            {
                // 輸入的帳號如果對應資料庫是 Null ?
                var NanAccount = _context.Users.Where(o => o.UserAccount == UserAccount).FirstOrDefault();
                if (NanAccount == null)
                {
                    HttpContext.Session.SetString("userToastr", "此帳號目前尚未誕生，請由您親手建立專屬自己的帳號吧！");
                    return Redirect("/Auth/Register");
                }

                // 將帳號和密碼對應到資料庫一模一樣的帳號密碼，以取得該 UserId
                var ac_pwd = (from u in _context.Users
                              where u.UserAccount == UserAccount
                              where u.UserPassword == UserPassword
                              select u.UserId).ToList()[0];

                // 取得 UserId 的登入判斷依據
                var get_UserId = _context.Users.Find(ac_pwd);
                int id = get_UserId.UserId;                     // 寫入Session用
                string ac = get_UserId.UserAccount.ToString();        // 取得該使用者帳號
                string pwd = get_UserId.UserPassword.ToString();      // 取得該使用者密碼
                string st = get_UserId.UserStatus.ToString();   // 取得該帳號狀態
                string isok = get_UserId.IsSuccess.ToString();  // 取得該帳號開通狀態

                // 多道判斷  是否有取得該使用者輸入的帳號、密碼
                if (ac != null && pwd != null)
                {
                    // 登入成功
                    if (isok == "True" && st == "NR")
                    {
                        HttpContext.Session.SetString("userToastr", "登入成功");
                        HttpContext.Session.SetString("userId", id.ToString());
                        return Redirect("/Auth/Index");
                    }
                    else if (isok == "false" && st == "NR")
                    {
                        HttpContext.Session.SetString("userToastr", $"<a href='/Auth/OneMoreEmailSending'>補寄驗證信 => 點我</a>");
                        return View();
                    }
                    else if (isok == "True" && st != "NR")
                    {
                        if (st == null)
                        {
                            HttpContext.Session.SetString("userToastr", "此帳號暫時無法使用，請聯繫客服");
                            return View();
                        }
                        else if (st == "SP")
                        {
                            HttpContext.Session.SetString("userToastr", "此帳號暫時無法使用，請聯繫客服");
                            return View();
                        }
                        else if (st == "WL")
                        {
                            HttpContext.Session.SetString("userToastr", "無效帳號");
                            return View();
                        }
                    }
                }
                HttpContext.Session.SetString("userToastr", "唉呀！出了點狀況···");
                return View();
            }
            HttpContext.Session.SetString("userToastr", "請輸入完整的帳號密碼");
            return View();
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++【 註冊 】+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        public async Task<IActionResult> Register(string account, string password, string confirmPassword, string email, string userStatus)
        {
            // 【正則表達式 Regex】 -------------------------------------------------------------------------------------
            //Regex regexAccount = new Regex(@"^[a-zA-Z0-9]{6,20}$");
            //Regex regexPassword = new Regex(@"^(?!.*[^\x21-\x7e])(?=.{6,20})(?=.*[a-zA-Z])(?=.*[a-zA-Z])(?=.*\d).*$");
            // ------------------- -------------------------------------------------------------------------------------

            // 判斷所有註冊需要輸入的欄位是否有空值或是回傳Null的情形
            if (!(string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email)))
            {
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ======> START <===== 因為以下欄位為第一次寫入，故尚未有資料，凡以下欄位皆須單獨做判斷 ======> START <======
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // ************************************ 取得輸入的帳號 ***************************************************
                var InputAC = _context.Users.Where(o => o.UserAccount == account);
                // ************************************ 取得用戶Email ****************************************************
                var InputEmail = _context.Users.Where(o => o.Email == email);
                // ************************************ 取得主密碼 *******************************************************
                var InputPwd = _context.Users.Where(o => o.UserPassword == password);
                // ************************************ 取得第二道驗證密碼 ************************************************
                var InputConfirmPwd = _context.Users.Where(o => o.ConfirmPassword == confirmPassword);
                // ************************************ 取得帳號目前狀態 **************************************************
                var UpdateStatus = _context.Users.Where(o => o.UserStatus == userStatus);
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


                if (!(password == confirmPassword || string.IsNullOrEmpty(InputAC.ToString()) || string.IsNullOrEmpty(InputEmail.ToString()) || string.IsNullOrEmpty(UpdateStatus.ToString())))
                {
                    // 1. -------------------------------------- 判斷帳號是否已被創建 -----------------------------------------
                    if (!string.IsNullOrEmpty(InputAC.ToString()))
                    {
                        HttpContext.Session.SetString("userToastr", "這組帳號有人使用囉！換一組帳號吧！");
                        return View();
                    }
                    // 2. -------------------------------------- 判斷前後密碼相符 ---------------------------------------------
                    if (password != confirmPassword)
                    {
                        HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                        return View();
                    }
                    // 3. -------------------------------------- 判斷信箱是否已被其他用戶使用 ----------------------------------
                    if (!string.IsNullOrEmpty(InputEmail.ToString()))
                    {
                        if (string.IsNullOrWhiteSpace(email))
                        {
                            HttpContext.Session.SetString("userToastr", "Email 請填空");
                            return View();
                        }
                        else
                        {
                            HttpContext.Session.SetString("userToastr", "換組 Email 吧！");
                            return View();
                        }
                    }
                    // 4. --------------------------------- 判斷用戶帳號使用權(還沒創建前都是Null) ------------------------------
                    if (!string.IsNullOrEmpty(UpdateStatus.ToString()) || (UpdateStatus.ToString() != "NR"))
                    {
                        if (userStatus == "SP")
                        {
                            HttpContext.Session.SetString("userToastr", "此帳號遭停權，請洽客服中心");
                            return View();
                        }
                        else if (userStatus == "WL")
                        {
                            HttpContext.Session.SetString("userToastr", "這是您的帳號嗎？");
                            return View();
                        }
                    }
                }

                // *********************************************************************************************************
                // *********************************************************************************************************
                // **********************【 完成以上判斷條件式，若是可以註冊，就執行以下程式碼片段 】****************************
                // *********************************************************************************************************            
                // *********************************************************************************************************

                // 註冊時，有些使用者的資料不一定要立即取得，但是仍然需要額外設定回傳值給資料庫
                User newMember = new User
                {
                    //UserId = userid,      // UserId => PK 自動編號
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
                return View("Login");
            }
            HttpContext.Session.SetString("userToastr", "幫幫我！把格子填完吧！");
            return View();
        }

        // =================================================================================================================================================================================
        // 【POST : 會員中心 】 =============================================================================================================================================================
        // =================================================================================================================================================================================

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++【 會員中心 => 基本資料變更 】+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        public async Task<IActionResult> MemberInfo(string email, string username, string phone, DateTime birthday, string region, string address)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u.UserId).FirstOrDefault();

            // 優先判斷 Users 表中的 UserId 是否對應到 Session 傳入的 userId
            if (userID.ToString() != userId)
            {
                HttpContext.Session.SetString("userToastr", "請重新輸入");
                return Redirect("/Auth/Login");
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
            HttpContext.Session.SetString("userToastr", "會員資料變更成功");

            return Redirect("/Auth/Index");
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++【 會員中心 => 密碼變更 】+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string NewPassword, string confirmNewPassword)
        {
            // 取得舊密碼
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

            _context.Update(changUserNewPwd);
            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("userToastr", "密碼變更成功，請重新登入");
            return Redirect("/Auth/Login");
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++【 會員中心 => 訂單變更 】+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        [HttpPost]
        public async Task<IActionResult> MemberOrderEdit(int id,[Bind("OrderId,UserId,OrderDate,PayMethod,TotalPrice,Approval,Address,UserName")] Order order)
        {
            //if (id != orderDetail.OrderId)
            //{
            //    return NotFound();
            //}

            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = _context.Users.Where(u => u.UserId.ToString() == userId).FirstOrDefault();

            // 判斷 userId 是否為訪客 || 判斷 Users 表中的 UserId 是否對應到 Session 傳入的 userId
            if (userId == "Guest" || userID.UserId.ToString() != userId || userId == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }


            var CancelOrder = (from u in _context.OrderDetails
                               where u.OrderId == id
                               select u.OrderId).FirstOrDefault();

            var f_order_user = (from u in _context.Users
                                where u.UserId == userID.UserId
                                select u.UserId).ToList()[0];

            // 設定該使用者之 UserId 用以變更
            var CancelOrderinfo = _context.Orders.Find(CancelOrder);
            CancelOrderinfo.Approval = "WL";   // 將訂單狀態變更為"取消" = "WL"

            _context.Update(CancelOrderinfo);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("userToastr", "已為您完成取消訂單");
            HttpContext.Session.SetString("userId", f_order_user.ToString());
            return View();
        }
    }
}

