using System;
using System.Collections.Generic;
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
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Auth/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // =======================================================================================









        // ============================== IUser_Services ==========================================
        // 登入
        // GET: /Auth/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // 註冊
        public async Task<IActionResult> RegisterUserAsync(User user)
        {
            User userConfirm = new User()
            {
                UserId = 0,
                Email = "",
                UserPassword = "",
                //ConfirmPassword = "",
                UserStatus = "SP",   // 帳號尚未開通 = "SP"
                //IsSuccess = false    // 啟用帳號 = false
            };

            if (user == null)
                throw new NullReferenceException("很抱歉，目前無法使用註冊功能");

            if (await _context.Users.FindAsync(user.Email) != null)
            {
                HttpContext.Session.SetString("userToastr", "帳號已存在");
                return View();
            }

            if (ModelState.IsValid)
            {

                //if (user.UserPassword != user.ConfirmPassword)
                //{

                //    HttpContext.Session.SetString("userToastr", "前後密碼不相符");
                //    return View();
                //}


                _context.Add(user);
                await _context.SaveChangesAsync();


                // 帳號完成開通 = "NR"
                userConfirm.UserStatus = "NR";
                // 啟用帳號 = true
                //userConfirm.IsSuccess = true;



            };
            HttpContext.Session.SetString("userToastr", "您現在可以至電子郵件收取會員註冊信以完成驗證程序");
            return Redirect("/Employee/Index");
        }


        // Email => 使用者點擊返回驗證模組
        public ActionResult ConfirmEmail()
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
            //changUserStatus.IsSuccess = true;   // IsSuccess 型態為 bool
            changUserStatus.UpdateDate = DateTime.Now;

            HttpContext.Session.SetString("userToastr", "會員已開通完成，現在請放心購物");
            _context.SaveChanges();
            return View();

        }


    }


    
}

