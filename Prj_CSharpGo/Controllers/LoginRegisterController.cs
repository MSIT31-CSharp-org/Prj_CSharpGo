using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prj_CSharpGo.Models;

namespace Prj_CSharpGo.Controllers
{
    public class LoginRegisterController : Controller
    {
        private readonly WildnessCampingContext _context;

        public LoginRegisterController(WildnessCampingContext context)
        {
            _context = context;
        }

        // 會員註冊
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User getuser)
        {
            // 未通過驗證，顯示目前的View
            if (ModelState.IsValid == false)
            {
                return View();
            }
            var Isuser = _context.Users.Where(u => u.UserAccount == getuser.UserAccount).FirstOrDefault();

            // 若 Isuser 為 null，表示會員尚未註冊
            if (Isuser == null)
            {
                // 將會員紀錄新增到 User 資料表
                _context.Users.Add(getuser);
                _context.SaveChanges();

                // 執行  控制器的 Login 動作
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號已有人使用";
            return View();

        }

        // 會員登入
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View();
            }
            // Session 的寫入
            HttpContext.Session.SetString("userName", userName);
            return Redirect("/Home/Index");
        }
        // 會員中心
        public IActionResult MemberCenter()
        {
            string userName = HttpContext.Session.GetString("userName") ?? "Guest";
            if (userName == "Guest")
            {
                return Redirect("/Home/Index");
            }
            return View();
        }
        // 會員登出
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userName");
            return Redirect("/Home/Index");
        }
    }
}
